namespace GenAIPlatform.Infrastructure.Retrieval;

internal static class PostgresRagSearchSql
{
    public static string GetDistanceExpression(int dimensions)
    {
        // These SQL fragments are hard-coded for known vector dimensions; no user text
        // is interpolated into the distance expression.
        return dimensions switch
        {
            16 => "chunk.embedding_vector::vector(16) <=> query_embedding.embedding::vector(16)",
            1536 => "chunk.embedding_vector::vector(1536) <=> query_embedding.embedding::vector(1536)",
            _ => "chunk.embedding_vector <=> query_embedding.embedding"
        };
    }
}
