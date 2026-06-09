using GenAIPlatform.Application.Core.Dispatching;

namespace GenAIPlatform.Application.Knowledge.Documents;

public sealed record GetDocumentStatusQuery(Guid DocumentId) : IRequest<DocumentStatusResponse?>;
