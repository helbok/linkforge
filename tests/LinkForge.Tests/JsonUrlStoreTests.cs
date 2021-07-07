using System;
using System.IO;
using System.Threading.Tasks;
using LinkForge.Core;
using LinkForge.Storage;
using Xunit;

namespace LinkForge.Tests;

public class JsonUrlStoreTests : IDisposable
{
    private readonly string _path = Path.Combine(
        Path.GetTempPath(), "linkforge-tests", Guid.NewGuid().ToString("N"), "links.json");

    [Fact]
    public async Task Put_then_get_round_trips_through_disk()
    {
        var store = new JsonUrlStore(_path);

        await store.PutAsync(new ShortLink
        {
            Code = "abc",
            Destination = "https://example.com",
            CreatedAt = DateTimeOffset.UtcNow
        });

        // Simulate a restart by creating a fresh store over the same file.
        var reopened = new JsonUrlStore(_path);

        var link = await reopened.GetAsync("abc");

        Assert.NotNull(link);
        Assert.Equal("https://example.com", link!.Destination);
    }

    [Fact]
    public async Task Get_missing_file_returns_null()
    {
        var store = new JsonUrlStore(_path);

        Assert.Null(await store.GetAsync("abc"));
    }

    public void Dispose()
    {
        string directory = Path.GetDirectoryName(_path)!;

        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, recursive: true);
        }
    }
}
