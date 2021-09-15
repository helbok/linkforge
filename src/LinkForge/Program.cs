using System.Threading.Tasks;
using LinkForge.Cli;
using LinkForge.Core;
using LinkForge.Storage;

namespace LinkForge;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var options = new ShortenerOptions
        {
            Store = StoreKind.Memory,
            BaseUrl = "https://linkforge.example"
        };

        IUrlStore store = UrlStoreFactory.Create(options);
        IIdEncoder encoder = new Base62Encoder();
        var service = new ShortenerService(encoder, store, options);
        var runner = new ConsoleRunner(service);

        return await runner.RunAsync(args).ConfigureAwait(false);
    }
}
