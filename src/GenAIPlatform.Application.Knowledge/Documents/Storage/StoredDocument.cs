namespace GenAIPlatform.Application.Knowledge.Documents;

public sealed record StoredDocument(
    string StoragePath,
    string ContentHash,
    long SizeBytes,
    string? StagedStoragePath = null);
