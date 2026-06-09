using GenAIPlatform.Domain.Documents;

namespace GenAIPlatform.Application.Knowledge.Documents;

public sealed record DocumentIndexingStatusSnapshot(
    Document Document,
    IndexingJob? LatestJob,
    int ChunkCount);
