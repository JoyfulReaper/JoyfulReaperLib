/*
 * JoyfulReaperLibrary
 * 
 *  Copyright (c) 2026 Kyle Givler
 * Licensed under the MIT License.
 */

using Microsoft.Data.Sqlite;
using System;
using System.Threading.Tasks;

namespace JoyfulReaperLib.JRData.Web;

public static class HitCountHelper
{
    private const string _visitorsSchema = @"
        CREATE TABLE IF NOT EXISTS Visitors (
                IpAddress TEXT PRIMARY KEY,
                Hits INTEGER DEFAULT 1,
                LastSeen TEXT
            );";

    private static async Task EnsureTableExists(SqliteConnection db)
    {
        await db.OpenAsync();
        using var cmd = db.CreateCommand();
        cmd.CommandText = _visitorsSchema;
        cmd.ExecuteNonQuery();
    }


    public async static Task<(long totalHits, long uniqueVisitors)> GetHitCounts(SqliteConnection db)
    {
        long totalHits = 0;
        long uniqueVisitors = 0;

        var statsCmd = db.CreateCommand();
        statsCmd.CommandText = "SELECT COUNT(IpAddress), SUM(Hits) FROM Visitors;";
        using var reader = await statsCmd.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            uniqueVisitors = reader.IsDBNull(0) ? 0 : reader.GetInt64(0);
            totalHits = reader.IsDBNull(1) ? 0 : reader.GetInt64(1);
        }

        return (totalHits, uniqueVisitors);
    }


    public async static Task<(long totalHits, long uniqueVisitors)> ProcessHitCounts(SqliteConnection db, string ip)
    {
        await db.OpenAsync();
        await EnsureTableExists(db);

        // Update the hit count
        var upsertCmd = db.CreateCommand();
        upsertCmd.CommandText = @"
            INSERT INTO Visitors (IpAddress, Hits, LastSeen)
            VALUES ($ip, 1, $date)
            ON CONFLICT(IpAddress) DO UPDATE SET
                Hits = Hits + 1,
                LastSeen = $date;
        ";
        upsertCmd.Parameters.AddWithValue("$ip", ip);
        upsertCmd.Parameters.AddWithValue("$date", DateTime.UtcNow.ToString("o"));
        await upsertCmd.ExecuteNonQueryAsync();

        // Get Totals
        var statsCmd = db.CreateCommand();
        statsCmd.CommandText = "SELECT COUNT(IpAddress), SUM(Hits) FROM Visitors;";
        using var reader = await statsCmd.ExecuteReaderAsync();

        return await GetHitCounts(db);
    }
}
