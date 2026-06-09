using GenAIPlatform.Domain.Documents;

namespace GenAIPlatform.Application.Knowledge.Documents;

public sealed record ValidatedDocumentUpload(
    string FileName,
    string Extension,
    DocumentAccessLevel AccessLevel);
