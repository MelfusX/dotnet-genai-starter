namespace GenAIPlatform.Domain.Observability;

public sealed record RetrievedDocumentReference(
    string ReferenceId,
    Guid DocumentId,
    Guid? ChunkId = null);
