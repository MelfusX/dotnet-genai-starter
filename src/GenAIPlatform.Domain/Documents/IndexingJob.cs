namespace GenAIPlatform.Domain.Documents;

public sealed record IndexingJob(
    Guid Id,
    Guid DocumentId,
    IndexingJobStatus Status,
    int Attempts,
    int MaxAttempts,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    DateTimeOffset AvailableAtUtc,
    DateTimeOffset? StartedAtUtc,
    DateTimeOffset? CompletedAtUtc,
    string? WorkerId,
    string? FailureReason);
