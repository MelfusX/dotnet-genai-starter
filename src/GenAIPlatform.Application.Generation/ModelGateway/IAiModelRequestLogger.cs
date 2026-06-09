using GenAIPlatform.Application.Core.ModelClients;
using GenAIPlatform.Domain.Observability;

namespace GenAIPlatform.Application.Generation.ModelGateway;

public interface IAiModelRequestLogger
{
    Task<AiModelResponse> CompleteAndLogAsync(
        IAiModelClient modelClient,
        AiModelRequest request,
        TimeSpan? retrievalLatency,
        int? embeddingTokens,
        string? embeddingProvider,
        string? embeddingModel,
        IReadOnlyList<RetrievedDocumentReference> retrievedDocuments,
        CancellationToken cancellationToken);

    Task LogSucceededWithoutModelAsync(
        string correlationId,
        string model,
        TimeSpan latency,
        int? embeddingTokens,
        string? embeddingProvider,
        string? embeddingModel,
        TimeSpan? retrievalLatency,
        IReadOnlyList<RetrievedDocumentReference> retrievedDocuments);
}
