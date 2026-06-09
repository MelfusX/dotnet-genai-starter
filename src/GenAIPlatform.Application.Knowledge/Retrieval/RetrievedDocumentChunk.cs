namespace GenAIPlatform.Application.Knowledge.Retrieval;

/// <summary>
/// A document chunk that has already passed retrieval authorization and embedding
/// compatibility filters and can be rendered into a RAG prompt.
/// </summary>
public sealed record RetrievedDocumentChunk(
    Guid DocumentId,
    Guid ChunkId,
    int DocumentVersion,
    int ChunkPosition,
    string Title,
    string FileName,
    string Text,
    double SimilarityScore);
