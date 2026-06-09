using GenAIPlatform.Application.Core.Dispatching;

namespace GenAIPlatform.Application.Knowledge.Documents;

public sealed record ProcessDocumentStorageCleanupCommand(
    string WorkerId,
    int? MaxRequests)
    : IRequest<ProcessDocumentStorageCleanupResponse>;
