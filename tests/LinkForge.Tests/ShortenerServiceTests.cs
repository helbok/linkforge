using LinkForge.Core;
using LinkForge.Storage;
using Xunit;

namespace LinkForge.Tests;

public class ShortenerServiceTests
{
    private readonly ShortenerService _service = CreateService();

    [Fact]
    public void Shorten_then_resolve_round_trips()
    {
        var link = _service.Shorten("https://example.com/hello");

        var resolved = _service.Resolve(link.Code);

        Assert.NotNull(resolved);
        Assert.Equal("https://example.com/hello", resolved!.Destination);
    }

    [Fact]
    public void Resolve_missing_code_returns_null()
    {
        Assert.Null(_service.Resolve("does-not-exist"));
    }

    [Fact]
    public void Shorten_same_destination_returns_same_code()
    {
        var first = _service.Shorten("https://example.com/dup");
        var second = _service.Shorten("https://example.com/dup");

        Assert.Equal(first.Code, second.Code);
    }

    [Fact]
    public void Shorten_rejects_invalid_url()
    {
        Assert.Throws<ArgumentException>(() => _service.Shorten("not-a-url"));
    }

    [Fact]
    public void Shorten_with_custom_code_uses_it()
    {
        var link = _service.Shorten("https://example.com", customCode: "home");

        Assert.Equal("home", link.Code);
    }

    [Fact]
    public void Shorten_rejects_duplicate_custom_code()
    {
        _service.Shorten("https://example.com/one", customCode: "home");

        Assert.Throws<InvalidOperationException>(
            () => _service.Shorten("https://example.com/two", customCode: "home"));
    }

    [Fact]
    public void Shorten_with_ttl_sets_expiry()
    {
        var link = _service.Shorten("https://example.com/ttl", ttl: TimeSpan.FromDays(30));

        Assert.NotNull(link.ExpiresAt);
        Assert.True(link.ExpiresAt > System.DateTimeOffset.UtcNow);
    }

    [Fact]
    public void Resolve_expired_link_returns_null()
    {
        var link = _service.Shorten("https://example.com/expired", ttl: TimeSpan.FromDays(-1));

        Assert.Null(_service.Resolve(link.Code));
    }

    [Fact]
    public void Resolve_increments_hit_count()
    {
        var link = _service.Shorten("https://example.com/hits");

        _service.Resolve(link.Code);
        _service.Resolve(link.Code);

        var stored = _service.Resolve(link.Code);
        Assert.NotNull(stored);
        Assert.Equal(3, stored!.Hits);
    }

    private static ShortenerService CreateService()
    {
        var options = new ShortenerOptions();
        var store = new InMemoryUrlStore();
        return new ShortenerService(new Base62Encoder(), store, options);
    }
}
