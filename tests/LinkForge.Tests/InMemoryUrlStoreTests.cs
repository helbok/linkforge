using LinkForge.Core;
using LinkForge.Storage;
using Xunit;

namespace LinkForge.Tests;

public class InMemoryUrlStoreTests
{
    private readonly InMemoryUrlStore _store = new InMemoryUrlStore();

    [Fact]
    public void Put_then_get_returns_the_link()
    {
        _store.Put(NewLink("abc"));

        var fetched = _store.Get("abc");

        Assert.NotNull(fetched);
        Assert.Equal("https://example.com", fetched!.Destination);
    }

    [Fact]
    public void Get_missing_code_returns_null()
    {
        Assert.Null(_store.Get("missing"));
    }

    [Fact]
    public void Remove_deletes_the_link()
    {
        _store.Put(NewLink("abc"));

        bool removed = _store.Remove("abc");

        Assert.True(removed);
        Assert.Null(_store.Get("abc"));
    }

    private static ShortLink NewLink(string code) => new ShortLink
    {
        Code = code,
        Destination = "https://example.com",
        CreatedAt = System.DateTimeOffset.UtcNow
    };
}
