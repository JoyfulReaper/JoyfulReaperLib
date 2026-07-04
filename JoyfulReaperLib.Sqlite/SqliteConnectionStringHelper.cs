/*
MIT License

Copyright(c) 2026 Kyle Givler
https://github.com/JoyfulReaper

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using Microsoft.Data.Sqlite;

namespace JoyfulReaperLib.Sqlite;

public static class SqliteConnectionStringHelper
{
    public static string Resolve(string connectionString, string? basePath = null)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("SQLite connection string must include a Data Source.");
        }

        var builder = new SqliteConnectionStringBuilder(connectionString);
        if (string.IsNullOrWhiteSpace(builder.DataSource))
        {
            throw new InvalidOperationException("SQLite connection string must include a Data Source.");
        }

        if (IsSpecialDataSource(builder.DataSource))
        {
            return builder.ToString();
        }

        if (!Path.IsPathRooted(builder.DataSource))
        {
            var resolvedBasePath = ResolveBasePath(basePath);
            Directory.CreateDirectory(resolvedBasePath);
            builder.DataSource = Path.GetFullPath(Path.Combine(resolvedBasePath, builder.DataSource));
            return builder.ToString();
        }

        builder.DataSource = Path.GetFullPath(builder.DataSource);
        var directory = Path.GetDirectoryName(builder.DataSource);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        return builder.ToString();
    }

    private static string ResolveBasePath(string? basePath)
    {
        if (string.IsNullOrWhiteSpace(basePath))
        {
            return Path.Combine(AppContext.BaseDirectory, "Data");
        }

        return Path.IsPathRooted(basePath)
            ? Path.GetFullPath(basePath)
            : Path.GetFullPath(basePath, AppContext.BaseDirectory);
    }

    private static bool IsSpecialDataSource(string dataSource)
        => string.Equals(dataSource, ":memory:", StringComparison.OrdinalIgnoreCase)
           || dataSource.StartsWith("file:", StringComparison.OrdinalIgnoreCase);
}
