using System.Collections.Concurrent;
using System.Collections.Generic;
using LinkForge.Core;

namespace LinkForge.Storage;

public sealed class InMemoryUrlStore : IUrlStore
{
    private readonly ConcurrentDictionary<string, ShortLink> _links =
        new ConcurrentDictionary<string, ShortLink>();

    public ShortLink? Get(string code)
    {
        _links.TryGetValue(code, out var link);
        return link;
    }

    public void Put(ShortLink link)
    {
        _links[link.Code] = link;
    }

    public bool Remove(string code)
    {
        return _links.TryRemove(code, out _);
    }

    public IReadOnlyList<ShortLink> List()
    {
        return new List<ShortLink>(_links.Values);
    }
}
