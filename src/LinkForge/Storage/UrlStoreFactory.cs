using System;
using LinkForge.Core;

namespace LinkForge.Storage;

public static class UrlStoreFactory
{
    public static IUrlStore Create(ShortenerOptions options)
    {
        if (options is null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        switch (options.Store)
        {
            case StoreKind.Memory:
                return new InMemoryUrlStore();

            case StoreKind.Json:
                return new JsonUrlStore(options.StorePath);

            default:
                throw new ArgumentOutOfRangeException(nameof(options), "Unknown store kind.");
        }
    }
}
