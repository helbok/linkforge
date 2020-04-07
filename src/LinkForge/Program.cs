using System;
using LinkForge.Cli;

namespace LinkForge;

internal static class Program
{
    private static int Main(string[] args)
    {
        var runner = new ConsoleRunner();
        return runner.Run(args);
    }
}
