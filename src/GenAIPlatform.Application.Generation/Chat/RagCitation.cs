namespace GenAIPlatform.Application.Generation.Chat;

public sealed record RagCitation(
    string ReferenceId,
    Guid DocumentId,
    Guid ChunkId,
    int DocumentVersion,
    int ChunkPosition,
    string Title,
    string FileName,
    double SimilarityScore);
