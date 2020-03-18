using System;
using System.Collections.Generic;

namespace LinkForge.Core;

public sealed class ShortenerService
{
    private readonly IIdEncoder _encoder;
    private readonly IUrlStore _store;
    private long _nextId = 1;

    public ShortenerService(IIdEncoder encoder, IUrlStore store)
    {
        _encoder = encoder ?? throw new ArgumentNullException(nameof(encoder));
        _store = store ?? throw new ArgumentNullException(nameof(store));
    }

    public ShortLink Shorten(string destination)
    {
        foreach (var link in _store.List())
        {
            if (string.Equals(link.Destination, destination, StringComparison.Ordinal))
            {
                return link;
            }
        }

        string code = _encoder.Encode(_nextId++);

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
}
