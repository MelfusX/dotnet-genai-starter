namespace GenAIPlatform.Application.Knowledge.Retrieval;

/// <summary>
/// Searches indexed document chunks that are safe to add to a RAG prompt.
/// Implementations must apply tenant, user access, document-id and embedding
/// compatibility filters before returning chunk text. If a filter cannot be
/// applied safely, implementations must fail closed rather than returning
/// unfiltered chunks. Embedding dimension mismatches must be excluded before
/// distance ranking and must not surface provider or database errors to callers.
/// A successful empty result means all filters were applied and no relevant
/// chunks matched. Invalid query embeddings, result limits, similarity
/// thresholds, document filters, infrastructure, schema or query failures must
/// be reported through <see cref="RagVectorSearchException" /> instead of
/// returning an empty no-context result.
/// </summary>
public interface IRagVectorSearchStore
{
    /// <summary>
    /// Verifies that the retrieval store is configured and has the required
    /// schema before provider-facing query embedding work starts. This is a
    /// readiness preflight, not a guarantee that a later search cannot fail
    /// transiently.
    /// </summary>
    Task CheckReadinessAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Returns chunks ordered by descending semantic similarity for the supplied
    /// query. Results must be limited to indexed documents in the requested
    /// tenant, tenant-public documents or private documents owned by the user,
    /// optional document IDs, and embeddings produced by the same provider and
    /// model as the query embedding. Implementations must fail closed when
    /// <see cref="RagVectorSearchQuery.TopK" />,
    /// <see cref="RagVectorSearchQuery.MinSimilarityScore" /> or
    /// <see cref="RagVectorSearchQuery.DocumentIds" /> are outside the query
    /// contract, including blank tenant/model/provider metadata and oversized
    /// document filters.
    /// </summary>
    Task<IReadOnlyList<RetrievedDocumentChunk>> SearchAsync(
        RagVectorSearchQuery query,
        CancellationToken cancellationToken);
}
