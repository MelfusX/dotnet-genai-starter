using GenAIPlatform.Application.Core.Dispatching;

namespace GenAIPlatform.Application.Knowledge.Documents;

public sealed record UploadDocumentCommand(
    string FileName,
    string? ContentType,
    long? Length,
    string? Title,
    string? AccessLevel,
    Stream Content)
    : IRequest<UploadDocumentResponse>;
