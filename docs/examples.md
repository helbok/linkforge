# Examples

## Custom codes

```console
$ linkforge shorten https://example.com/about --code about
```

## Links that expire

```console
$ linkforge shorten https://example.com/promo --ttl 30
```

## Library use with a JSON store

```csharp
var options = new ShortenerOptions
{
    Store = StoreKind.Json,
    StorePath = "links.json"
};

IUrlStore store = UrlStoreFactory.Create(options);
var service = new ShortenerService(new Base62Encoder(), store, options);
```
