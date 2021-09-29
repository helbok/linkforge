using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LinkForge.Core;
using Microsoft.Data.Sqlite;

namespace LinkForge.Storage;

public sealed class SqliteUrlStore : IUrlStore, IDisposable
{
    private readonly SqliteConnection _connection;

    public SqliteUrlStore(string connectionString)
    {
        _connection = new SqliteConnection(connectionString);
        _connection.Open();
        InitializeSchema();
    }

    private void InitializeSchema()
    {
        using var command = _connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS links (
                code        TEXT PRIMARY KEY,
                destination TEXT NOT NULL,
                created_at  TEXT NOT NULL,
                expires_at  TEXT NULL,
                hits        INTEGER NOT NULL DEFAULT 0
            );";
        command.ExecuteNonQuery();
    }

    public async Task<ShortLink?> GetAsync(string code)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT destination, created_at, expires_at, hits FROM links WHERE code = $code;";
        command.Parameters.AddWithValue("$code", code);

        using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

        if (!await reader.ReadAsync().ConfigureAwait(false))
        {
            return null;
        }

        return new ShortLink
        {
            Code = code,
            Destination = reader.GetString(0),
            CreatedAt = DateTimeOffset.Parse(reader.GetString(1)),
            ExpiresAt = reader.IsDBNull(2) ? null : DateTimeOffset.Parse(reader.GetString(2)),
            Hits = reader.GetInt64(3)
        };
    }

    public async Task PutAsync(ShortLink link)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO links (code, destination, created_at, expires_at, hits)
            VALUES ($code, $destination, $created_at, $expires_at, $hits)
            ON CONFLICT(code) DO UPDATE SET
                destination = excluded.destination,
                expires_at  = excluded.expires_at,
                hits        = excluded.hits;";
        command.Parameters.AddWithValue("$code", link.Code);
        command.Parameters.AddWithValue("$destination", link.Destination);
        command.Parameters.AddWithValue("$created_at", link.CreatedAt.ToString("O"));
        command.Parameters.AddWithValue("$expires_at", (object?)link.ExpiresAt?.ToString("O") ?? DBNull.Value);
        command.Parameters.AddWithValue("$hits", link.Hits);

        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
    }

    public async Task<bool> RemoveAsync(string code)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = "DELETE FROM links WHERE code = $code;";
        command.Parameters.AddWithValue("$code", code);

        return await command.ExecuteNonQueryAsync().ConfigureAwait(false) > 0;
    }

    public async Task<IReadOnlyList<ShortLink>> ListAsync()
    {
        var links = new List<ShortLink>();

        using var command = _connection.CreateCommand();
        command.CommandText = "SELECT code, destination, created_at, expires_at, hits FROM links;";

        using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

        while (await reader.ReadAsync().ConfigureAwait(false))
        {
            links.Add(new ShortLink
            {
                Code = reader.GetString(0),
                Destination = reader.GetString(1),
                CreatedAt = DateTimeOffset.Parse(reader.GetString(2)),
                ExpiresAt = reader.IsDBNull(3) ? null : DateTimeOffset.Parse(reader.GetString(3)),
                Hits = reader.GetInt64(4)
            });
        }

        return links;
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}
