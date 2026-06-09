using Npgsql;
using NpgsqlTypes;

namespace GenAIPlatform.Infrastructure.Evaluations;

internal static class PostgresEvaluationParameters
{
    public static void Add(
        NpgsqlCommand command,
        string name,
        object? value)
    {
        command.Parameters.AddWithValue(name, value ?? DBNull.Value);
    }

    public static void AddJson(
        NpgsqlCommand command,
        string name,
        string value)
    {
        command.Parameters.AddWithValue(name, NpgsqlDbType.Jsonb, value);
    }
}
