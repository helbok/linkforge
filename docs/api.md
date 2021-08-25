# API Reference

LinkForge exposes a small library API in addition to its command-line tool.

## Core types

### `IIdEncoder`

Converts a numeric id to/from a short code.

- `string Encode(long id)`
- `long Decode(string code)`

### `IUrlStore`

Persistence contract for `ShortLink` records.

- `Task<ShortLink?> GetAsync(string code)`
- `Task PutAsync(ShortLink link)`
- `Task<bool> RemoveAsync(string code)`
- `Task<IReadOnlyList<ShortLink>> ListAsync()`

### `ShortenerService`

The main entry point for shortening and resolving links.

```csharp
var options = new ShortenerOptions { Store = StoreKind.Json, StorePath = "links.json" };
IUrlStore store = UrlStoreFactory.Create(options);
var service = new ShortenerService(new Base62Encoder(), store, options);

ShortLink link = await service.ShortenAsync("https://example.com");
ShortLink? resolved = await service.ResolveAsync(link.Code);
```

## Storage backends

| Backend  | `StoreKind` | Notes                        |
|----------|-------------|------------------------------|
| Memory   | `Memory`    | Default; data lost on exit.  |
| JSON file| `Json`      | Human-readable, single file. |
| SQLite   | `Sqlite`    | For larger, indexed datasets.|
