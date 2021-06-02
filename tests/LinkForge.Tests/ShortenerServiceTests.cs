using System;
using System.Threading.Tasks;
using LinkForge.Core;
using LinkForge.Storage;
using Xunit;

namespace LinkForge.Tests;

public class ShortenerServiceTests
{
    private readonly ShortenerService _service = CreateService();

    [Fact]
    public async Task Shorten_then_resolve_round_trips()
    {
        var link = await _service.ShortenAsync("https://example.com/hello");

        var resolved = await _service.ResolveAsync(link.Code);

        Assert.NotNull(resolved);
        Assert.Equal("https://example.com/hello", resolved!.Destination);
    }

    [Fact]
    public async Task Resolve_missing_code_returns_null()
    {
        Assert.Null(await _service.ResolveAsync("does-not-exist"));
    }

    [Fact]
    public async Task Shorten_same_destination_returns_same_code()
    {
        var first = await _service.ShortenAsync("https://example.com/dup");
        var second = await _service.ShortenAsync("https://example.com/dup");

        Assert.Equal(first.Code, second.Code);
    }

    [Fact]
    public async Task Shorten_rejects_invalid_url()
    {
        await Assert.ThrowsAsync<ArgumentException>(
            () => _service.ShortenAsync("not-a-url"));
    }

    [Fact]
    public async Task Shorten_with_custom_code_uses_it()
    {
        var link = await _service.ShortenAsync("https://example.com", customCode: "home");

        Assert.Equal("home", link.Code);
    }

    [Fact]
    public async Task Shorten_rejects_duplicate_custom_code()
    {
        await _service.ShortenAsync("https://example.com/one", customCode: "home");

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _service.ShortenAsync("https://example.com/two", customCode: "home"));
    }

    [Fact]
    public async Task Shorten_with_ttl_sets_expiry()
    {
        var link = await _service.ShortenAsync(
            "https://example.com/ttl", ttl: TimeSpan.FromDays(30));

        Assert.NotNull(link.ExpiresAt);
        Assert.True(link.ExpiresAt > System.DateTimeOffset.UtcNow);
    }

    [Fact]
    public async Task Resolve_expired_link_returns_null()
    {
        var link = await _service.ShortenAsync(
            "https://example.com/expired", ttl: TimeSpan.FromDays(-1));

        Assert.Null(await _service.ResolveAsync(link.Code));
    }

    [Fact]
    public async Task Resolve_increments_hit_count()
    {
        var link = await _service.ShortenAsync("https://example.com/hits");

        await _service.ResolveAsync(link.Code);
        await _service.ResolveAsync(link.Code);

        var stored = await _service.ResolveAsync(link.Code);
        Assert.NotNull(stored);
        Assert.Equal(3, stored!.Hits);
    }

    [Fact]
    public async Task List_excludes_expired_links()
    {
        await _service.ShortenAsync("https://example.com/alive");
        await _service.ShortenAsync("https://example.com/dead", ttl: TimeSpan.FromDays(-1));

        var links = await _service.ListAsync();

        var link = Assert.Single(links);
        Assert.Equal("https://example.com/alive", link.Destination);
    }

    private static ShortenerService CreateService()
    {
        var options = new ShortenerOptions();
        var store = new InMemoryUrlStore();
        return new ShortenerService(new Base62Encoder(), store, options);
    }
}
