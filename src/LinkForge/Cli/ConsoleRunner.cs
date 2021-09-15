using System;
using System.Threading.Tasks;
using LinkForge.Core;

namespace LinkForge.Cli;

public sealed class ConsoleRunner
{
    private readonly ShortenerService _service;

    public ConsoleRunner(ShortenerService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async Task<int> RunAsync(string[] args)
    {
        try
        {
            return await ExecuteAsync(ArgumentParser.Parse(args)).ConfigureAwait(false);
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

    private async Task<int> ExecuteAsync(Command command)
    {
        switch (command)
        {
            case ShortenCommand shorten:
                return await ExecuteShortenAsync(shorten).ConfigureAwait(false);

            case ResolveCommand resolve:
                return await ExecuteResolveAsync(resolve).ConfigureAwait(false);

            case ListCommand:
                return await ExecuteListAsync().ConfigureAwait(false);

            default:
                PrintUsage();
                return 0;
        }
    }

    private async Task<int> ExecuteShortenAsync(ShortenCommand shorten)
    {
        var ttl = shorten.TtlDays.HasValue
            ? TimeSpan.FromDays(shorten.TtlDays.Value)
            : (TimeSpan?)null;

        var link = await _service.ShortenAsync(shorten.Url, shorten.CustomCode, ttl)
            .ConfigureAwait(false);

        Console.WriteLine($"Shortened: {shorten.Url}");
        Console.WriteLine($"Code:      {link.Code}");

        if (link.ExpiresAt.HasValue)
        {
            Console.WriteLine($"Expires:   {link.ExpiresAt:O}");
        }

        return 0;
    }

    private async Task<int> ExecuteResolveAsync(ResolveCommand resolve)
    {
        var resolved = await _service.ResolveAsync(resolve.Code).ConfigureAwait(false);

        if (resolved is null)
        {
            Console.Error.WriteLine($"No link found for code '{resolve.Code}'.");
            return 1;
        }

        Console.WriteLine(resolved.Destination);
        return 0;
    }

    private async Task<int> ExecuteListAsync()
    {
        var links = await _service.ListAsync().ConfigureAwait(false);

        foreach (var link in links)
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
        Console.WriteLine("  linkforge shorten <url> [--code <code>] [--ttl <days>]");
        Console.WriteLine("  linkforge resolve <code>");
        Console.WriteLine("  linkforge list");
        Console.WriteLine("  linkforge help");
    }
}
