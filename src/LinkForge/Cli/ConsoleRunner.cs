using System;
using LinkForge.Core;
using LinkForge.Storage;

namespace LinkForge.Cli;

public sealed class ConsoleRunner
{
    private readonly ShortenerService _service;

    public ConsoleRunner()
    {
        var store = new InMemoryUrlStore();
        var encoder = new Base62Encoder();
        _service = new ShortenerService(encoder, store);
    }

    public int Run(string[] args)
    {
        try
        {
            Command command = ArgumentParser.Parse(args);

            switch (command)
            {
                case ShortenCommand shorten:
                    var link = _service.Shorten(shorten.Url);
                    Console.WriteLine($"Shortened: {shorten.Url}");
                    Console.WriteLine($"Code:      {link.Code}");
                    return 0;

                case ResolveCommand resolve:
                    var resolved = _service.Resolve(resolve.Code);

                    if (resolved is null)
                    {
                        Console.Error.WriteLine($"No link found for code '{resolve.Code}'.");
                        return 1;
                    }

                    Console.WriteLine(resolved.Destination);
                    return 0;

                case ListCommand:
                    foreach (var link in _service.List())
                    {
                        Console.WriteLine($"{link.Code}\t{link.Destination}");
                    }
                    return 0;

                default:
                    PrintUsage();
                    return 0;
            }
        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            Console.Error.WriteLine("Run 'linkforge help' for usage.");
            return 2;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Unexpected error: {ex.Message}");
            return 3;
        }
    }

    private static void PrintUsage()
    {
        Console.WriteLine("LinkForge — a small URL shortener");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  linkforge shorten <url>");
        Console.WriteLine("  linkforge resolve <code>");
        Console.WriteLine("  linkforge list");
        Console.WriteLine("  linkforge help");
    }
}
