using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LinkForge.Core;

public sealed class ShortenerService
{
    private readonly IIdEncoder _encoder;
    private readonly IUrlStore _store;
    private readonly ShortenerOptions _options;
    private long _nextId = 1;

    public ShortenerService(IIdEncoder encoder, IUrlStore store, ShortenerOptions options)
    {
        _encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<ShortLink> ShortenAsync(
        string destination,
        string? customCode = null,
        TimeSpan? ttl = null,
        CancellationToken ct = default)
    {
        UrlValidator.Validate(destination, _options.MaxUrlLength);

        if (!string.IsNullOrEmpty(customCode))
        {
            ValidateCustomCode(customCode);

            if (await _store.GetAsync(customCode).ConfigureAwait(false) != null)
            {
                throw new InvalidOperationException($"Code '{customCode}' is already in use.");
            }
        }

        foreach (var link in await _store.ListAsync().ConfigureAwait(false))
        {
            if (string.Equals(link.Destination, destination, StringComparison.Ordinal))
            {
                return link;
            }
        }

        string code = customCode ?? _encoder.Encode(_nextId++);

        var created = new ShortLink
        {
            Code = code,
            Destination = destination,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = ttl.HasValue ? DateTimeOffset.UtcNow.Add(ttl.Value) : (DateTimeOffset?)null
        };

        await _store.PutAsync(created).ConfigureAwait(false);

        return created;
    }

    public async Task<ShortLink?> ResolveAsync(string code, CancellationToken ct = default)
    {
        var link = await _store.GetAsync(code).ConfigureAwait(false);

        if (link == null || link.IsExpiredAt(DateTimeOffset.UtcNow))
        {
            return null;
        }

        link.Hits += 1;
        await _store.PutAsync(link).ConfigureAwait(false);

        return link;
    }

    public async Task<IReadOnlyList<ShortLink>> ListAsync(CancellationToken ct = default)
    {
        var links = await _store.ListAsync().ConfigureAwait(false);

        return links
            .Where(l => !l.IsExpiredAt(DateTimeOffset.UtcNow))
            .ToList();
    }

    private static void ValidateCustomCode(string code)
    {
        if (code.Length > 32)
        {
            throw new ArgumentException(
                "Custom codes must be at most 32 characters long.", nameof(code));
        }

        foreach (char c in code)
        {
            if (!char.IsLetterOrDigit(c) && c != '-' && c != '_')
            {
                throw new ArgumentException(
                    "Custom codes may only contain letters, digits, '-' and '_'.", nameof(code));
            }
        }
    }
}
