using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinkForge.Core;

namespace LinkForge.Storage;

public sealed class InMemoryUrlStore : IUrlStore
{
    private readonly ConcurrentDictionary<string, ShortLink> _links =
        new ConcurrentDictionary<string, ShortLink>();

    public Task<ShortLink?> GetAsync(string code)
    {
        _links.TryGetValue(code, out var link);
        return Task.FromResult(link);
    }

    public Task PutAsync(ShortLink link)
    {
        _links[link.Code] = link;
        return Task.CompletedTask;
    }

    public Task<bool> RemoveAsync(string code)
    {
        return Task.FromResult(_links.TryRemove(code, out _));
    }

    public Task<IReadOnlyList<ShortLink>> ListAsync()
    {
        return Task.FromResult<IReadOnlyList<ShortLink>>(
            new List<ShortLink>(_links.Values));
    }
}
