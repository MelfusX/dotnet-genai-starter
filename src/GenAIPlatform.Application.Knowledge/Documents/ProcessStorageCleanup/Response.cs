namespace GenAIPlatform.Application.Knowledge.Documents;

public sealed record ProcessDocumentStorageCleanupResponse(
    int Discovered,
    int Deleted,
    int Deferred,
    int Failed);
