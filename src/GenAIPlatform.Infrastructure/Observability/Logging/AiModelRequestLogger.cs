using GenAIPlatform.Application.Core.ModelClients;
using GenAIPlatform.Application.Generation.ModelGateway;
using GenAIPlatform.Domain.Observability;

namespace GenAIPlatform.Infrastructure.Observability.Logging;

internal sealed class AiModelRequestLogger(
    AiModelRequestLoggingService requestLoggingService)
    : IAiModelRequestLogger
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
        return requestLoggingService.CompleteAndLogAsync(
            modelClient,
            request,
            retrievalLatency,
            embeddingTokens,
            embeddingProvider,
            embeddingModel,
            retrievedDocuments,
            cancellationToken);
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
        return requestLoggingService.LogSucceededWithoutModelAsync(
            correlationId,
            model,
            latency,
            embeddingTokens,
            embeddingProvider,
            embeddingModel,
            retrievalLatency,
            retrievedDocuments);
    }
}
