using GenAIPlatform.Application.Core.ModelClients;
using GenAIPlatform.Domain.Observability;

namespace GenAIPlatform.Application.Generation.ModelGateway;

internal sealed class NoopAiModelRequestLogger : IAiModelRequestLogger
{
    public Task<AiModelResponse> CompleteAndLogAsync(
        IAiModelClient modelClient,
        AiModelRequest request,
        TimeSpan? retrievalLatency,
        int? embeddingTokens,
        string? embeddingProvider,
        string? embeddingModel,
        IReadOnlyList<RetrievedDocumentReference> retrievedDocuments,
        CancellationToken cancellationToken)
    {
        return modelClient.CompleteAsync(request, cancellationToken);
    }

    public Task LogSucceededWithoutModelAsync(
        string correlationId,
        string model,
        TimeSpan latency,
        int? embeddingTokens,
        string? embeddingProvider,
        string? embeddingModel,
        TimeSpan? retrievalLatency,
        IReadOnlyList<RetrievedDocumentReference> retrievedDocuments)
    {
        return Task.CompletedTask;
    }
}
