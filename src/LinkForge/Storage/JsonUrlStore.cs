using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LinkForge.Core;

namespace LinkForge.Storage;

public sealed class JsonUrlStore : IUrlStore
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    private readonly string _path;
    private readonly SemaphoreSlim _gate = new SemaphoreSlim(1, 1);

    public JsonUrlStore(string path)
    {
        _path = path ?? throw new ArgumentNullException(nameof(path));
    }

    private async Task<Dictionary<string, ShortLink>> LoadAsync(CancellationToken ct = default)
    {
        if (!File.Exists(_path))
        {
            return new Dictionary<string, ShortLink>();
        }

        string json = await File.ReadAllTextAsync(_path, ct).ConfigureAwait(false);
        var links = JsonSerializer.Deserialize<Dictionary<string, ShortLink>>(json, Options);

        return links ?? new Dictionary<string, ShortLink>();
    }

    private async Task SaveAsync(Dictionary<string, ShortLink> links, CancellationToken ct = default)
    {
        string json = JsonSerializer.Serialize(links, Options);
        string directory = Path.GetDirectoryName(_path);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string tempPath = _path + ".tmp";
        await File.WriteAllTextAsync(tempPath, json, ct).ConfigureAwait(false);
        File.Move(tempPath, _path, overwrite: true);
    }

    public async Task<ShortLink?> GetAsync(string code)
    {
        var links = await LoadAsync().ConfigureAwait(false);
        links.TryGetValue(code, out var link);
        return link;
    }

    public async Task PutAsync(ShortLink link)
    {
        await _gate.WaitAsync().ConfigureAwait(false);
        try
        {
            var links = await LoadAsync().ConfigureAwait(false);
            links[link.Code] = link;
            await SaveAsync(links).ConfigureAwait(false);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<bool> RemoveAsync(string code)
    {
        await _gate.WaitAsync().ConfigureAwait(false);
        try
        {
            var links = await LoadAsync().ConfigureAwait(false);
            bool removed = links.Remove(code);

            if (removed)
            {
                await SaveAsync(links).ConfigureAwait(false);
            }

            return removed;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<IReadOnlyList<ShortLink>> ListAsync()
    {
        var links = await LoadAsync().ConfigureAwait(false);
        return new List<ShortLink>(links.Values);
    }
}
