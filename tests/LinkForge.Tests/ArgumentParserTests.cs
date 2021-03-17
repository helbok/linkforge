using System;
using LinkForge.Cli;
using Xunit;

namespace LinkForge.Tests;

public class ArgumentParserTests
{
    [Fact]
    public void Parse_with_no_args_returns_help()
    {
        Assert.IsType<HelpCommand>(ArgumentParser.Parse(Array.Empty<string>()));
    }

    [Fact]
    public void Parse_shorten_returns_shorten_command()
    {
        var command = Assert.IsType<ShortenCommand>(
            ArgumentParser.Parse(new[] { "shorten", "https://example.com" }));

        Assert.Equal("https://example.com", command.Url);
    }

    [Fact]
    public void Parse_shorten_reads_custom_code()
    {
        var command = Assert.IsType<ShortenCommand>(
            ArgumentParser.Parse(new[] { "shorten", "https://example.com", "--code", "home" }));

        Assert.Equal("home", command.CustomCode);
    }

    [Fact]
    public void Parse_shorten_reads_ttl()
    {
        var command = Assert.IsType<ShortenCommand>(
            ArgumentParser.Parse(new[] { "shorten", "https://example.com", "--ttl", "30" }));

        Assert.Equal(30, command.TtlDays);
    }

    [Fact]
    public void Parse_shorten_without_url_throws()
    {
        Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(new[] { "shorten" }));
    }

    [Fact]
    public void Parse_resolve_returns_resolve_command()
    {
        var command = Assert.IsType<ResolveCommand>(
            ArgumentParser.Parse(new[] { "resolve", "abc" }));

        Assert.Equal("abc", command.Code);
    }

    [Fact]
    public void Parse_list_returns_list_command()
    {
        Assert.IsType<ListCommand>(ArgumentParser.Parse(new[] { "list" }));
    }

    [Fact]
    public void Parse_unknown_command_throws()
    {
        Assert.Throws<ArgumentException>(() => ArgumentParser.Parse(new[] { "frobnicate" }));
    }
}
