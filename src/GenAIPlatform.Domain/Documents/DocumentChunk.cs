namespace GenAIPlatform.Domain.Documents;

public sealed record DocumentChunk(
    Guid Id,
    Guid DocumentId,
    int DocumentVersion,
    int Position,
    string Text,
    string TextHash,
    int ApproximateTokenCount,
    string ChunkingProfile,
    string ChunkingProfileVersion,
    IReadOnlyList<float> Embedding,
    string EmbeddingModel,
    string EmbeddingProvider,
    int? EmbeddingInputTokens,
    DateTimeOffset CreatedAtUtc)
{
    public int EmbeddingDimensions => Embedding.Count;
}
