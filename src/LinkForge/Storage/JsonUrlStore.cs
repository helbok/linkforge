using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using LinkForge.Core;

namespace LinkForge.Storage;

public sealed class JsonUrlStore : IUrlStore
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    private readonly string _path;

    public JsonUrlStore(string path)
    {
        _path = path ?? throw new ArgumentNullException(nameof(path));
    }

    private Dictionary<string, ShortLink> Load()
    {
        if (!File.Exists(_path))
        {
            return new Dictionary<string, ShortLink>();
        }

        string json = File.ReadAllText(_path);
        var links = JsonSerializer.Deserialize<Dictionary<string, ShortLink>>(json, Options);

        return links ?? new Dictionary<string, ShortLink>();
    }

    private void Save(Dictionary<string, ShortLink> links)
    {
        string json = JsonSerializer.Serialize(links, Options);

        // Bug (fixed in the next commit): writes directly to the target file,
        // so a crash mid-write can leave a truncated, corrupt store.
        File.WriteAllText(_path, json);
    }

    public ShortLink? Get(string code)
    {
        var links = Load();
        links.TryGetValue(code, out var link);
        return link;
    }

    public void Put(ShortLink link)
    {
        var links = Load();
        links[link.Code] = link;
        Save(links);
    }

    public bool Remove(string code)
    {
        var links = Load();
        bool removed = links.Remove(code);

        if (removed)
        {
            Save(links);
        }

        return removed;
    }

    public IReadOnlyList<ShortLink> List()
    {
        var links = Load();
        return new List<ShortLink>(links.Values);
    }
}
