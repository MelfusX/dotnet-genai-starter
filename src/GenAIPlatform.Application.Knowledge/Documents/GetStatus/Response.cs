namespace GenAIPlatform.Application.Knowledge.Documents;

public sealed record DocumentStatusResponse(
    Guid DocumentId,
    string Title,
    string FileName,
    int Version,
    string AccessLevel,
    string IndexingStatus,
    Guid? IndexingJobId,
    string? IndexingJobStatus,
    int IndexingAttempts,
    int ChunkCount,
    string? FailureReason,
    DateTimeOffset UpdatedAtUtc);
