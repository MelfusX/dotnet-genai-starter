using GenAIPlatform.Application.Core.Dispatching;

namespace GenAIPlatform.Application.Knowledge.Documents;

internal sealed class UploadDocumentHandler(DocumentUploadWorkflow workflow)
    : IRequestHandler<UploadDocumentCommand, UploadDocumentResponse>
{
    public async Task<UploadDocumentResponse> HandleAsync(
        UploadDocumentCommand request,
        CancellationToken cancellationToken)
    {
        return await workflow.HandleAsync(
            request,
            cancellationToken);
    }
}
