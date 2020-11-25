using System;
using System.Collections.Generic;

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

    public ShortLink Shorten(string destination, string? customCode = null)
    {
        UrlValidator.Validate(destination, _options.MaxUrlLength);

        if (!string.IsNullOrEmpty(customCode))
        {
            ValidateCustomCode(customCode);

            if (_store.Get(customCode) != null)
            {
                throw new InvalidOperationException($"Code '{customCode}' is already in use.");
            }
        }

        foreach (var link in _store.List())
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
            CreatedAt = DateTimeOffset.UtcNow
        };

        _store.Put(created);

        return created;
    }

    public ShortLink? Resolve(string code)
    {
        return _store.Get(code);
    }

    public IReadOnlyList<ShortLink> List()
    {
        return _store.List();
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
