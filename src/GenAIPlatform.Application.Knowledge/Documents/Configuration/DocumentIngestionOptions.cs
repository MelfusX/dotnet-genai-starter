namespace GenAIPlatform.Application.Knowledge.Documents;

public sealed class DocumentIngestionOptions
{
    public const string SectionName = "GenAIPlatform:DocumentIngestion";

    public long MaxUploadBytes { get; init; } = 2 * 1024 * 1024;

    public IReadOnlyCollection<string> AllowedExtensions { get; init; } = [".txt", ".md"];

    public int ChunkMaxCharacters { get; init; } = 1200;

    public int ChunkOverlapCharacters { get; init; } = 150;

    public string ChunkingProfile { get; init; } = "plain-text";

    public string ChunkingProfileVersion { get; init; } = "v1";

    public int MaxIndexingAttempts { get; init; } = 3;

    public int IndexingRetryDelaySeconds { get; init; } = 30;

    public int MaxStorageCleanupAttempts { get; init; } = 3;

    public int StorageCleanupRetryDelaySeconds { get; init; } = 30;

    public int MaxIndexingJobsPerPoll { get; init; } = 5;

    public int MaxStorageCleanupRequestsPerPoll { get; init; } = 5;

    public int ProcessingJobLeaseSeconds { get; init; } = 900;

    public int WorkerPollIntervalSeconds { get; init; } = 30;
}
