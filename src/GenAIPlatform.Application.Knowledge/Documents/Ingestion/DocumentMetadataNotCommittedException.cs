namespace GenAIPlatform.Application.Knowledge.Documents;

public sealed class DocumentMetadataNotCommittedException : Exception
{
    public DocumentMetadataNotCommittedException(
        Guid documentId,
        string message,
        Exception? innerException = null)
        : base(message, innerException)
    {
        DocumentId = documentId;
    }

    public Guid DocumentId { get; }
}
