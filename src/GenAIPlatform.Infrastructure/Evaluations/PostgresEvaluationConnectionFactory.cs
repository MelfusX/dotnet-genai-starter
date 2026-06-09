using GenAIPlatform.Infrastructure.Postgres;
using Npgsql;

namespace GenAIPlatform.Infrastructure.Evaluations;

internal sealed class PostgresEvaluationConnectionFactory(PostgresDataSourceProvider dataSourceProvider)
{
    public async Task<NpgsqlConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        return await dataSourceProvider.OpenConnectionAsync(cancellationToken);
    }
}
