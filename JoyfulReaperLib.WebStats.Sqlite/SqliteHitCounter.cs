using System.Data;
using System.Globalization;
using JoyfulReaperLib.Sqlite;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace JoyfulReaperLib.WebStats.Sqlite;

public sealed class SqliteHitCounter : IHitCounter
{
    private readonly string _connectionString;
    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private bool _initialized;

    public SqliteHitCounter(IOptions<SqliteHitCounterOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        SqliteProviderInitializer.Initialize();
        _connectionString = SqliteConnectionStringHelper.Resolve(options.Value.ConnectionString);
    }

    public async Task<HitCountStats> GetHitCountsAsync(CancellationToken ct = default)
    {
        await EnsureInitializedAsync(ct);

        await using var connection = CreateConnection();
        await connection.OpenAsync(ct);

        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(IpAddress), SUM(Hits) FROM Visitors;";

        await using var reader = await command.ExecuteReaderAsync(CommandBehavior.SingleRow, ct);
        if (!await reader.ReadAsync(ct))
        {
            return new HitCountStats(0, 0);
        }

        var uniqueVisitors = reader.IsDBNull(0) ? 0 : reader.GetInt64(0);
        var totalHits = reader.IsDBNull(1) ? 0 : reader.GetInt64(1);

        return new HitCountStats(totalHits, uniqueVisitors);
    }

    public async Task<HitCountStats> RecordHitAsync(string visitorKey, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(visitorKey);

        await EnsureInitializedAsync(ct);

        await using var connection = CreateConnection();
        await connection.OpenAsync(ct);

        await using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO Visitors (IpAddress, Hits, LastSeen)
            VALUES ($visitorKey, 1, $lastSeen)
            ON CONFLICT(IpAddress) DO UPDATE SET
                Hits = Hits + 1,
                LastSeen = $lastSeen;
            """;
        command.Parameters.AddWithValue("$visitorKey", visitorKey.Trim());
        command.Parameters.AddWithValue(
            "$lastSeen",
            DateTimeOffset.UtcNow.ToString("O", CultureInfo.InvariantCulture));

        await command.ExecuteNonQueryAsync(ct);
        return await GetHitCountsAsync(ct);
    }

    private async Task EnsureInitializedAsync(CancellationToken ct)
    {
        if (_initialized)
        {
            return;
        }

        await _initializationLock.WaitAsync(ct);
        try
        {
            if (_initialized)
            {
                return;
            }

            await using var connection = CreateConnection();
            await connection.OpenAsync(ct);

            await using var command = connection.CreateCommand();
            command.CommandText = """
                CREATE TABLE IF NOT EXISTS Visitors (
                    IpAddress TEXT PRIMARY KEY,
                    Hits INTEGER NOT NULL DEFAULT 1,
                    LastSeen TEXT
                );
                """;

            await command.ExecuteNonQueryAsync(ct);
            _initialized = true;
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    private SqliteConnection CreateConnection()
        => new(_connectionString);
}
