namespace GenAIPlatform.Application.Knowledge.Documents;

public sealed record UploadDocumentResponse(
    Guid DocumentId,
    string Title,
    string FileName,
    int Version,
    string AccessLevel,
    string IndexingStatus,
    Guid IndexingJobId,
    string IndexingJobStatus,
    DateTimeOffset CreatedAtUtc);
