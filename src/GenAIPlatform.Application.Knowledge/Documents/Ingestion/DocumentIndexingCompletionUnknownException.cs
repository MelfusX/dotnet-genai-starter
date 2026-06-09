namespace GenAIPlatform.Application.Knowledge.Documents;

public sealed class DocumentIndexingCompletionUnknownException : Exception
{
    public DocumentIndexingCompletionUnknownException(
        Guid documentId,
        Guid indexingJobId,
        string message,
        Exception? innerException = null)
        : base(message, innerException)
    {
        DocumentId = documentId;
        IndexingJobId = indexingJobId;
    }

    public Guid DocumentId { get; }

    public Guid IndexingJobId { get; }
}
