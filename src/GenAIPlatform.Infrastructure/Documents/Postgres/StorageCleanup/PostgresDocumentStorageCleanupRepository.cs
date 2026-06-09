using GenAIPlatform.Application.Knowledge.Documents;
using GenAIPlatform.Infrastructure.Documents.Postgres.Ingestion;

namespace GenAIPlatform.Infrastructure.Documents.Postgres.StorageCleanup;

internal sealed class PostgresDocumentStorageCleanupRepository
    : IDocumentStorageCleanupRepository
{
    private readonly PostgresDocumentStorageCleanupClaimStore claimStore;
    private readonly PostgresDocumentStorageCleanupRecordStore recordStore;
    private readonly PostgresDocumentStorageCleanupTransitionStore transitionStore;

    public PostgresDocumentStorageCleanupRepository(
        PostgresDocumentIngestionConnectionFactory connectionFactory)
    {
        recordStore = new PostgresDocumentStorageCleanupRecordStore(connectionFactory);
        claimStore = new PostgresDocumentStorageCleanupClaimStore(connectionFactory);
        transitionStore = new PostgresDocumentStorageCleanupTransitionStore(connectionFactory);
    }

    public async Task RecordAsync(
        DocumentStorageCleanupRequest request,
        CancellationToken cancellationToken)
    {
        await recordStore.RecordAsync(request, cancellationToken);
    }

    public async Task<IReadOnlyCollection<DocumentStorageCleanupRequest>> ClaimBatchAsync(
        string workerId,
        int maxRequests,
        TimeSpan processingLeaseDuration,
        CancellationToken cancellationToken)
    {
        return await claimStore.ClaimBatchAsync(
            workerId,
            maxRequests,
            processingLeaseDuration,
            cancellationToken);
    }

    public async Task<bool> CompleteAsync(
        DocumentStorageCleanupRequest request,
        CancellationToken cancellationToken)
    {
        return await transitionStore.CompleteAsync(request, cancellationToken);
    }

    public async Task<bool> DeferAsync(
        DocumentStorageCleanupRequest request,
        string failureReason,
        TimeSpan retryDelay,
        CancellationToken cancellationToken)
    {
        return await transitionStore.DeferAsync(
            request,
            failureReason,
            retryDelay,
            cancellationToken);
    }

    public async Task<bool> FailAsync(
        DocumentStorageCleanupRequest request,
        string failureReason,
        CancellationToken cancellationToken)
    {
        return await transitionStore.FailAsync(
            request,
            failureReason,
            cancellationToken);
    }
}
