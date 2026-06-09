namespace GenAIPlatform.Domain.Documents;

public sealed record Document(
    Guid Id,
    string TenantId,
    string OwnerUserId,
    string FileName,
    string Title,
    string? ContentType,
    string SourceExtension,
    string StoragePath,
    long SizeBytes,
    string ContentHash,
    int Version,
    DocumentAccessLevel AccessLevel,
    DocumentIndexingStatus IndexingStatus,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    string? FailureReason);
