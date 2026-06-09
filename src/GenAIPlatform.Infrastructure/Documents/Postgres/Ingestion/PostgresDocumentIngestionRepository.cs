using GenAIPlatform.Application.Knowledge.Documents;
using GenAIPlatform.Domain.Documents;
using GenAIPlatform.Infrastructure.Documents.Postgres.IndexingJobs;
using GenAIPlatform.Infrastructure.Documents.Postgres.Metadata;

namespace GenAIPlatform.Infrastructure.Documents.Postgres.Ingestion;

internal sealed class PostgresDocumentIngestionRepository
    : IDocumentIngestionRepository
{
    private readonly PostgresDocumentMetadataStore metadataStore;
    private readonly PostgresDocumentStatusReader statusReader;
    private readonly PostgresIndexingJobClaimStore claimStore;
    private readonly PostgresIndexingJobLeaseStore leaseStore;
    private readonly PostgresIndexingJobFailureStore failureStore;
    private readonly PostgresIndexingJobCompletionStore completionStore;

    public PostgresDocumentIngestionRepository(
        PostgresDocumentIngestionConnectionFactory connectionFactory)
    {
        var schemaReadiness = new PostgresIndexingSchemaReadiness();
        var jobLock = new PostgresIndexingJobLock();

        metadataStore = new PostgresDocumentMetadataStore(connectionFactory);
        statusReader = new PostgresDocumentStatusReader(connectionFactory);
        claimStore = new PostgresIndexingJobClaimStore(
            connectionFactory,
            schemaReadiness);
        leaseStore = new PostgresIndexingJobLeaseStore(connectionFactory);
        failureStore = new PostgresIndexingJobFailureStore(
            connectionFactory,
            jobLock);
        completionStore = new PostgresIndexingJobCompletionStore(
            connectionFactory,
            schemaReadiness,
            jobLock);
    }

    public async Task CreateDocumentWithJobAsync(
        Document document,
        IndexingJob indexingJob,
        CancellationToken cancellationToken)
    {
        await metadataStore.CreateDocumentWithJobAsync(
            document,
            indexingJob,
            cancellationToken);
    }

    public async Task<bool> DocumentExistsAsync(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        return await metadataStore.DocumentExistsAsync(
            documentId,
            cancellationToken);
    }

    public async Task<Document?> GetDocumentForIndexingAsync(
        Guid documentId,
        CancellationToken cancellationToken)
    {
        return await metadataStore.GetDocumentForIndexingAsync(
            documentId,
            cancellationToken);
    }

    public async Task<DocumentIndexingStatusSnapshot?> GetDocumentStatusAsync(
        Guid documentId,
        string tenantId,
        string? userId,
        CancellationToken cancellationToken)
    {
        return await statusReader.GetDocumentStatusAsync(
            documentId,
            tenantId,
            userId,
            cancellationToken);
    }

    public async Task<IndexingJob?> ClaimNextPendingJobAsync(
        string workerId,
        TimeSpan processingLeaseDuration,
        CancellationToken cancellationToken)
    {
        return await claimStore.ClaimNextPendingJobAsync(
            workerId,
            processingLeaseDuration,
            cancellationToken);
    }

    public async Task<int> MarkExpiredIndexingJobsFailedAsync(
        TimeSpan processingLeaseDuration,
        CancellationToken cancellationToken)
    {
        return await claimStore.MarkExpiredIndexingJobsFailedAsync(
            processingLeaseDuration,
            cancellationToken);
    }

    public async Task<bool> RenewProcessingLeaseAsync(
        Guid documentId,
        IndexingJob indexingJob,
        CancellationToken cancellationToken)
    {
        return await leaseStore.RenewProcessingLeaseAsync(
            documentId,
            indexingJob,
            cancellationToken);
    }

    public async Task<bool> ReplaceChunksAndCompleteIndexingAsync(
        Document document,
        IndexingJob indexingJob,
        IReadOnlyCollection<DocumentChunk> chunks,
        CancellationToken cancellationToken)
    {
        return await completionStore.ReplaceChunksAndCompleteIndexingAsync(
            document,
            indexingJob,
            chunks,
            cancellationToken);
    }

    public async Task<bool> MarkIndexingFailedAsync(
        Guid documentId,
        IndexingJob indexingJob,
        string failureReason,
        bool retry,
        TimeSpan retryDelay,
        CancellationToken cancellationToken)
    {
        return await failureStore.MarkIndexingFailedAsync(
            documentId,
            indexingJob,
            failureReason,
            retry,
            retryDelay,
            cancellationToken);
    }

    public async Task<bool> ReleaseProcessingJobAndRefundAttemptAsync(
        Guid documentId,
        IndexingJob indexingJob,
        CancellationToken cancellationToken)
    {
        return await leaseStore.ReleaseProcessingJobAndRefundAttemptAsync(
            documentId,
            indexingJob,
            cancellationToken);
    }
}
