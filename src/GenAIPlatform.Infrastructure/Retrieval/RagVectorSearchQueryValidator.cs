using GenAIPlatform.Application.Knowledge.Retrieval;

namespace GenAIPlatform.Infrastructure.Retrieval;

internal sealed class RagVectorSearchQueryValidator
{
    public void EnsureValid(RagVectorSearchQuery? query)
    {
        EnsureValidShape(query);
        EnsureSearchableEmbedding(query!.QueryEmbedding);
    }

    private static void EnsureValidShape(RagVectorSearchQuery? query)
    {
        if (query is null)
        {
            throw RagVectorSearchErrorMapper.InvalidQuery("RAG retrieval query is required.");
        }

        if (string.IsNullOrWhiteSpace(query.TenantId))
        {
            throw RagVectorSearchErrorMapper.InvalidQuery("RAG retrieval query tenant is invalid.");
        }

        if (string.IsNullOrWhiteSpace(query.EmbeddingModel))
        {
            throw RagVectorSearchErrorMapper.InvalidQuery("RAG retrieval query embedding model is invalid.");
        }

        if (string.IsNullOrWhiteSpace(query.EmbeddingProvider))
        {
            throw RagVectorSearchErrorMapper.InvalidQuery("RAG retrieval query embedding provider is invalid.");
        }

        if (query.TopK is < RagVectorSearchQuery.MinTopK or > RagVectorSearchQuery.MaxTopK)
        {
            throw RagVectorSearchErrorMapper.InvalidQuery("RAG retrieval query result limit is invalid.");
        }

        if (double.IsNaN(query.MinSimilarityScore) ||
            double.IsInfinity(query.MinSimilarityScore) ||
            query.MinSimilarityScore is < RagVectorSearchQuery.MinSimilarityScoreValue or
                > RagVectorSearchQuery.MaxSimilarityScoreValue)
        {
            throw RagVectorSearchErrorMapper.InvalidQuery("RAG retrieval query similarity threshold is invalid.");
        }

        if (query.DocumentIds is null)
        {
            throw RagVectorSearchErrorMapper.InvalidQuery("RAG retrieval query document filter is invalid.");
        }

        if (query.DocumentIds.Count > RagVectorSearchQuery.MaxDocumentFilters)
        {
            throw RagVectorSearchErrorMapper.InvalidQuery("RAG retrieval query document filter has too many values.");
        }

        if (query.DocumentIds.Any(static documentId => documentId == Guid.Empty))
        {
            throw RagVectorSearchErrorMapper.InvalidQuery("RAG retrieval query document filter is invalid.");
        }
    }

    private static void EnsureSearchableEmbedding(IReadOnlyList<float>? vector)
    {
        if (vector is null ||
            vector.Count == 0 ||
            !HasPositiveFiniteMagnitude(vector))
        {
            throw RagVectorSearchErrorMapper.InvalidQuery("RAG retrieval query embedding is invalid.");
        }
    }

    private static bool HasPositiveFiniteMagnitude(IReadOnlyList<float> vector)
    {
        var hasNonZeroValue = false;
        foreach (var value in vector)
        {
            if (!float.IsFinite(value))
            {
                return false;
            }

            hasNonZeroValue |= value != 0f;
        }

        return hasNonZeroValue;
    }
}
