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
    public string? CustomCode { get; set; }
    public int? TtlDays { get; set; }
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
            throw new ArgumentException("Usage: shorten <url> [--code <code>] [--ttl <days>]");
        }

        var command = new ShortenCommand { Url = args[1] };

        for (int i = 2; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--code":
                    command.CustomCode = RequireValue(args, ref i, "--code");
                    break;

                case "--ttl":
                    string value = RequireValue(args, ref i, "--ttl");

                    if (!int.TryParse(value, out int days) || days <= 0)
                    {
                        throw new ArgumentException("--ttl expects a positive number of days.");
                    }

                    command.TtlDays = days;
                    break;

                default:
                    throw new ArgumentException($"Unknown option '{args[i]}'.");
            }
        }

        return command;
    }

    private static Command ParseResolve(string[] args)
    {
        if (args.Length < 2)
        {
            throw new ArgumentException("Usage: resolve <code>");
        }

        return new ResolveCommand { Code = args[1] };
    }

    private static string RequireValue(string[] args, ref int index, string option)
    {
        if (index + 1 >= args.Length)
        {
            throw new ArgumentException($"Option {option} requires a value.");
        }

        index++;
        return args[index];
    }
}
