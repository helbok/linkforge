# LinkForge

A small URL shortener library and command-line tool, written in C#.

LinkForge turns long URLs into short, reversible codes using a base-62
encoding, and stores the mapping in a pluggable store.

## Features

- Base-62 short codes that encode a monotonically increasing id.
- Pluggable storage via `IUrlStore` — in-memory and JSON-file backends.
- A command-line interface for shortening, resolving, and listing links.

## Usage

```console
$ linkforge shorten https://example.com/a-very-long/path
Shortened: https://example.com/a-very-long/path
Code:      1

$ linkforge resolve 1
https://example.com/a-very-long/path

$ linkforge list
1   https://example.com/a-very-long/path
```

The command-line tool currently uses in-memory storage. The JSON-file
store is available to library consumers and keeps links across restarts.

## Architecture

```
src/LinkForge/
├── Core/        domain logic: encoding, storage contract, shortener service
├── Storage/     store implementations (in-memory, JSON file)
└── Cli/         command-line parsing and dispatch
```

- `IIdEncoder` — converts a numeric id to/from a short code.
- `IUrlStore` — persistence contract for `ShortLink` records.
- `ShortenerService` — orchestrates shortening and resolving.

## Documentation

- [API reference](docs/api.md)
- [Examples](docs/examples.md)

## Building

The project targets .NET 5.

```console
$ dotnet build
$ dotnet test
```
