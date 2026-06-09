namespace GenAIPlatform.Application.Knowledge.Documents;

internal enum DocumentUploadRollbackState
{
    StorageNotCommitted,
    RepositoryCreateNotStarted,
    MetadataNotCommitted,
    MetadataOutcomeUnknown
}
