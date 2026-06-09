namespace GenAIPlatform.Application.Knowledge.Retrieval;

/// <summary>
/// Describes a permission-aware vector search request. <paramref name="DocumentIds" />
/// is an additional metadata filter, not an authorization override. Implementations
/// must compare <paramref name="QueryEmbedding" /> only with stored embeddings that
/// match <paramref name="EmbeddingProvider" />, <paramref name="EmbeddingModel" />
/// and vector dimensions. <paramref name="TopK" /> must be between
/// <see cref="MinTopK" /> and <see cref="MaxTopK" />, and
/// <paramref name="MinSimilarityScore" /> must be finite and between
/// <see cref="MinSimilarityScoreValue" /> and <see cref="MaxSimilarityScoreValue" />.
/// <paramref name="TenantId" />, <paramref name="EmbeddingModel" /> and
/// <paramref name="EmbeddingProvider" /> must be nonblank.
/// <paramref name="DocumentIds" /> must be non-null, contain no empty GUID
/// values and contain at most <see cref="MaxDocumentFilters" /> values.
/// </summary>
public sealed record RagVectorSearchQuery(
    IReadOnlyList<float> QueryEmbedding,
    string EmbeddingModel,
    string EmbeddingProvider,
    string TenantId,
    string? UserId,
    int TopK,
    double MinSimilarityScore,
    IReadOnlyCollection<Guid> DocumentIds)
{
    public const int MinTopK = 1;
    public const int MaxTopK = 50;
    public const int MaxDocumentFilters = 100;
    public const double MinSimilarityScoreValue = -1;
    public const double MaxSimilarityScoreValue = 1;
}
