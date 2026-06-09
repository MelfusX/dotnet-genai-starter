using Npgsql;

namespace GenAIPlatform.IntegrationTests;

internal static class PostgresSchemaTestHelper
{
    public static async Task EnsureSchemaAsync(string connectionString)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        foreach (var scriptPath in Directory
                     .GetFiles(FindInitScriptDirectory(), "*.sql")
                     .Order(StringComparer.OrdinalIgnoreCase))
        {
            await ExecuteScriptAsync(connection, scriptPath);
        }
    }

    public static async Task ApplyInitScriptsAsync(
        string connectionString,
        params string[] scriptNames)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();

        var initScriptDirectory = FindInitScriptDirectory();
        foreach (var scriptName in scriptNames)
        {
            var scriptPath = Path.Combine(initScriptDirectory, scriptName);
            await ExecuteScriptAsync(connection, scriptPath);
        }
    }

    private static async Task ExecuteScriptAsync(
        NpgsqlConnection connection,
        string scriptPath)
    {
        var schemaSql = await File.ReadAllTextAsync(scriptPath);
        await using var command = new NpgsqlCommand(schemaSql, connection);
        await command.ExecuteNonQueryAsync();
    }

    private static string FindInitScriptDirectory()
    {
        foreach (var startPath in new[] { Environment.CurrentDirectory, AppContext.BaseDirectory })
        {
            var directory = new DirectoryInfo(startPath);
            while (directory is not null)
            {
                var candidate = Path.Combine(
                    directory.FullName,
                    "infra",
                    "postgres",
                    "init");
                if (Directory.Exists(candidate))
                {
                    return candidate;
                }

                directory = directory.Parent;
            }
        }

        throw new InvalidOperationException("PostgreSQL init script directory was not found.");
    }
}
