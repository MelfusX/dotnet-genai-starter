using GenAIPlatform.Application.Knowledge.Documents;
using Npgsql;

namespace GenAIPlatform.Infrastructure.Documents.Postgres.IndexingJobs;

internal sealed class PostgresIndexingSchemaReadiness
{
    private const string IndexingSchemaNotReadyMessage =
        "PostgreSQL document indexing schema is not ready. Apply infra/postgres/init/001-enable-pgvector.sql, 002-document-ingestion.sql and 003-pgvector-retrieval.sql before running indexing workers.";

    public async Task EnsureReadyAsync(
        NpgsqlConnection connection,
        CancellationToken cancellationToken)
    {
        await using var command = new NpgsqlCommand("""
            SELECT
                EXISTS (
                    SELECT 1
                    FROM pg_extension
                    WHERE extname = 'vector'
                ) AS has_vector_extension,
                to_regclass('genai.document_chunks') IS NOT NULL AS has_document_chunks_table,
                EXISTS (
                    SELECT 1
                    FROM information_schema.columns
                    WHERE table_schema = 'genai'
                      AND table_name = 'document_chunks'
                      AND column_name = 'embedding_vector'
                ) AS has_embedding_vector_column;
            """, connection);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken) ||
            !reader.GetBoolean(0) ||
            !reader.GetBoolean(1) ||
            !reader.GetBoolean(2))
        {
            throw new DocumentIndexingSchemaNotReadyException(IndexingSchemaNotReadyMessage);
        }
    }
}
