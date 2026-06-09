namespace GenAIPlatform.Application.Knowledge.Documents;

public interface IDocumentStorageCleanupRepository
{
    /// <summary>
    /// Durably records storage that was safe to delete during rollback but could not be deleted.
    /// Callers must invoke this only after proving that no durable document metadata exists for
    /// the storage identity.
    /// </summary>
    Task RecordAsync(
        DocumentStorageCleanupRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Atomically claims cleanup requests that are available for processing.
    /// Implementations must prevent duplicate worker ownership for the same request.
    /// </summary>
    Task<IReadOnlyCollection<DocumentStorageCleanupRequest>> ClaimBatchAsync(
        string workerId,
        int maxRequests,
        TimeSpan processingLeaseDuration,
        CancellationToken cancellationToken);

    /// <summary>
    /// Marks an owned cleanup request complete after storage content has been deleted, or verified absent.
    /// </summary>
    Task<bool> CompleteAsync(
        DocumentStorageCleanupRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Releases an owned cleanup request for a later retry when metadata still exists.
    /// </summary>
    Task<bool> DeferAsync(
        DocumentStorageCleanupRequest request,
        string failureReason,
        TimeSpan retryDelay,
        CancellationToken cancellationToken);

    /// <summary>
    /// Marks an owned cleanup request failed after deterministic validation or cleanup failure.
    /// </summary>
    Task<bool> FailAsync(
        DocumentStorageCleanupRequest request,
        string failureReason,
        CancellationToken cancellationToken);
}
