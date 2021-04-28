using System.Collections.Generic;
using System.Threading.Tasks;

namespace LinkForge.Core;

public interface IUrlStore
{
    Task<ShortLink?> GetAsync(string code);

    Task PutAsync(ShortLink link);

    Task<bool> RemoveAsync(string code);

    Task<IReadOnlyList<ShortLink>> ListAsync();
}
