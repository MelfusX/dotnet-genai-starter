namespace GenAIPlatform.Application.Knowledge.Documents;

public interface IDocumentStorage
{
    /// <summary>
    /// Saves content into a non-public/staged location and returns the final storage identity.
    /// If this method throws before returning, implementations must remove any staged or
    /// readable content created for this save attempt, or the adapter must document a durable
    /// reconciler that owns cleanup of that partial state.
    /// </summary>
    Task<StoredDocument> SaveAsync(
        Guid documentId,
        string fileName,
        Stream content,
        long maxSizeBytes,
        CancellationToken cancellationToken);

    /// <summary>
    /// Makes a saved document durably readable through <see cref="StoredDocument.StoragePath" />.
    /// Implementations must throw if the document is not committed.
    /// </summary>
    Task CommitAsync(
        StoredDocument document,
        CancellationToken cancellationToken);

    /// <summary>
    /// Opens only committed documents owned by this storage adapter.
    /// </summary>
    Task<Stream> OpenReadAsync(
        string storagePath,
        CancellationToken cancellationToken);

    /// <summary>
    /// Deletes a committed or staged document owned by this storage adapter.
    /// </summary>
    Task DeleteAsync(
        string storagePath,
        CancellationToken cancellationToken);
}
