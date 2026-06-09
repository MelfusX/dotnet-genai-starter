namespace GenAIPlatform.Application.Knowledge.Documents;

public sealed class DocumentStorageCleanupRecordingException : Exception
{
    public DocumentStorageCleanupRecordingException(
        Guid documentId,
        string storagePath,
        string metadataAbsenceProof,
        Exception deleteException,
        Exception recordingException)
        : base(
            "Document storage cleanup could not be recorded after upload rollback.",
            new AggregateException(
                "Rollback deletion failed and orphaned cleanup recording failed.",
                deleteException,
                recordingException))
    {
        DocumentId = documentId;
        StoragePath = storagePath;
        MetadataAbsenceProof = metadataAbsenceProof;
    }

    public Guid DocumentId { get; }

    public string StoragePath { get; }

    public string MetadataAbsenceProof { get; }
}
