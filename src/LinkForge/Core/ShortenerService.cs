using System;

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
        string code = _encoder.Encode(_nextId++);

        var link = new ShortLink
        {
            Code = code,
            Destination = destination,
            CreatedAt = DateTimeOffset.UtcNow
        };

        _store.Put(link);

        return link;
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
