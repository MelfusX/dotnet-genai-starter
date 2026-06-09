using GenAIPlatform.Application.Knowledge.Retrieval;
using GenAIPlatform.Infrastructure.Postgres;
using Npgsql;

namespace GenAIPlatform.Infrastructure.Retrieval;

internal sealed class PostgresRagConnectionFactory(PostgresDataSourceProvider dataSourceProvider)
{
    public const string ProviderName = "postgres";

    public async Task<NpgsqlConnection> OpenConnectionAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await dataSourceProvider.OpenConnectionAsync(cancellationToken);
        }
        catch (PostgresConnectionConfigurationException exception) when (exception.IsMissing)
        {
            throw new RagVectorSearchException(
                ProviderName,
                "RAG retrieval store is not configured.",
                errorCode: "retrieval_unavailable");
        }
        catch (PostgresConnectionConfigurationException exception)
        {
            throw new RagVectorSearchException(
                ProviderName,
                "RAG retrieval store is unavailable.",
                errorCode: "retrieval_unavailable",
                exception);
        }
    }
}
