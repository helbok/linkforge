using System;
using LinkForge.Core;
using LinkForge.Storage;

namespace LinkForge.Cli;

public sealed class ConsoleRunner
{
    private readonly ShortenerService _service;

    public ConsoleRunner()
    {
        var options = new ShortenerOptions();
        var store = UrlStoreFactory.Create(options);
        var encoder = new Base62Encoder();
        _service = new ShortenerService(encoder, store, options);
    }

    public int Run(string[] args)
    {
        try
        {
            return Execute(ArgumentParser.Parse(args));
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

    private int Execute(Command command)
    {
        switch (command)
        {
            case ShortenCommand shorten:
                return ExecuteShorten(shorten);

            case ResolveCommand resolve:
                return ExecuteResolve(resolve);

            case ListCommand:
                return ExecuteList();

            default:
                PrintUsage();
                return 0;
        }
    }

    private int ExecuteShorten(ShortenCommand shorten)
    {
        var ttl = shorten.TtlDays.HasValue
            ? TimeSpan.FromDays(shorten.TtlDays.Value)
            : (TimeSpan?)null;

        var link = _service.Shorten(shorten.Url, shorten.CustomCode, ttl);
        Console.WriteLine($"Shortened: {shorten.Url}");
        Console.WriteLine($"Code:      {link.Code}");

        if (link.ExpiresAt.HasValue)
        {
            Console.WriteLine($"Expires:   {link.ExpiresAt:O}");
        }

        return 0;
    }

    private int ExecuteResolve(ResolveCommand resolve)
    {
        var resolved = _service.Resolve(resolve.Code);

        if (resolved is null)
        {
            Console.Error.WriteLine($"No link found for code '{resolve.Code}'.");
            return 1;
        }

        Console.WriteLine(resolved.Destination);
        return 0;
    }

    private int ExecuteList()
    {
        foreach (var link in _service.List())
        {
            Console.WriteLine($"{link.Code}\t{link.Hits}\t{link.Destination}");
        }
        return 0;
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
