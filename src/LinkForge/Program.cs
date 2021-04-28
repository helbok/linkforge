using System.Threading.Tasks;
using LinkForge.Cli;

namespace LinkForge;

internal static class Program
{
    private static async Task<int> Main(string[] args)
    {
        var runner = new ConsoleRunner();
        return await runner.RunAsync(args).ConfigureAwait(false);
    }
}
