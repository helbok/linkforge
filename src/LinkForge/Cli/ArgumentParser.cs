using System;

namespace LinkForge.Cli;

public abstract class Command
{
    public abstract string Name { get; }
}

public sealed class ShortenCommand : Command
{
    public override string Name => "shorten";
    public string Url { get; set; } = string.Empty;
}

public sealed class ResolveCommand : Command
{
    public override string Name => "resolve";
    public string Code { get; set; } = string.Empty;
}

public sealed class ListCommand : Command
{
    public override string Name => "list";
}

public sealed class HelpCommand : Command
{
    public override string Name => "help";
}

public static class ArgumentParser
{
    public static Command Parse(string[] args)
    {
        if (args.Length == 0)
        {
            return new HelpCommand();
        }

        switch (args[0].ToLowerInvariant())
        {
            case "shorten":
                return ParseShorten(args);

            case "resolve":
                return ParseResolve(args);

            case "list":
                return new ListCommand();

            case "help":
            case "--help":
            case "-h":
                return new HelpCommand();

            default:
                throw new ArgumentException($"Unknown command '{args[0]}'.");
        }
    }

    private static Command ParseShorten(string[] args)
    {
        if (args.Length < 2)
        {
            throw new ArgumentException("Usage: shorten <url>");
        }

        return new ShortenCommand { Url = args[1] };
    }

    private static Command ParseResolve(string[] args)
    {
        if (args.Length < 2)
        {
            throw new ArgumentException("Usage: resolve <code>");
        }

        return new ResolveCommand { Code = args[1] };
    }
}
