using System.Threading.Tasks;
using LinkForge.Core;
using LinkForge.Storage;
using Xunit;

namespace LinkForge.Tests;

public class InMemoryUrlStoreTests
{
    private readonly InMemoryUrlStore _store = new InMemoryUrlStore();

    [Fact]
    public async Task Put_then_get_returns_the_link()
    {
        await _store.PutAsync(NewLink("abc"));

        var fetched = await _store.GetAsync("abc");

        Assert.NotNull(fetched);
        Assert.Equal("https://example.com", fetched!.Destination);
    }

    [Fact]
    public async Task Get_missing_code_returns_null()
    {
        Assert.Null(await _store.GetAsync("missing"));
    }

    [Fact]
    public async Task Remove_deletes_the_link()
    {
        await _store.PutAsync(NewLink("abc"));

        bool removed = await _store.RemoveAsync("abc");

        Assert.True(removed);
        Assert.Null(await _store.GetAsync("abc"));
    }

    private static ShortLink NewLink(string code) => new ShortLink
    {
        Code = code,
        Destination = "https://example.com",
        CreatedAt = System.DateTimeOffset.UtcNow
    };
}
