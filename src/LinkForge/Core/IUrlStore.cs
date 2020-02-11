using System.Collections.Generic;

namespace LinkForge.Core;

public interface IUrlStore
{
    ShortLink? Get(string code);

    void Put(ShortLink link);

    bool Remove(string code);

    IReadOnlyList<ShortLink> List();
}
