/*
 * JoyfulReaperLibrary
 * 
 *  Copyright (c) 2026 Kyle Givler
 * Licensed under the MIT License.
 */

using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace JoyfulReaperLib.JRData;

public class SqliteHelper
{
    public static string InitializeSqlite(string dbFileName, string? schemaSql)
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var dataFolder = Path.Combine(baseDirectory, "Data");
        Directory.CreateDirectory(dataFolder);

        var dbPath = Path.Combine(dataFolder, dbFileName);
        var connectionString = $"Data Source={dbPath};Mode=ReadWriteCreate;Cache=Shared;";

        if (schemaSql is not null)
        {
            using var connection = new SqliteConnection(connectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = schemaSql;
            command.ExecuteNonQuery();
        }

        return connectionString;
    }
}