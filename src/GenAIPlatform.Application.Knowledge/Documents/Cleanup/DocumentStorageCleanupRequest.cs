namespace GenAIPlatform.Application.Knowledge.Documents;

public sealed record DocumentStorageCleanupRequest(
    Guid DocumentId,
    string StoragePath,
    string? StagedStoragePath,
    string ContentHash,
    long SizeBytes,
    string MetadataAbsenceProof,
    DateTimeOffset MetadataAbsenceVerifiedAtUtc,
    string DeleteFailureReason,
    DocumentStorageCleanupStatus Status = DocumentStorageCleanupStatus.Pending,
    int Attempts = 0,
    DateTimeOffset? AvailableAtUtc = null,
    DateTimeOffset? CreatedAtUtc = null,
    DateTimeOffset? UpdatedAtUtc = null,
    string? WorkerId = null,
    string? FailureReason = null);
