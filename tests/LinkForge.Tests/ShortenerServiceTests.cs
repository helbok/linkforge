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

    private static ShortenerService CreateService()
    {
        var store = new InMemoryUrlStore();
        return new ShortenerService(new Base62Encoder(), store);
    }
}
