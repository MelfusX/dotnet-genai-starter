namespace GenAIPlatform.Mcp.Tools.Schemas;

public sealed record RagAnswerCitation(
    string ReferenceId,
    Guid DocumentId,
    Guid ChunkId,
    int DocumentVersion,
    int ChunkPosition,
    string Title,
    string FileName,
    double SimilarityScore);
