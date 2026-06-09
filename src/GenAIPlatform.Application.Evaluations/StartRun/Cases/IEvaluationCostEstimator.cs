using GenAIPlatform.Application.Core.ModelClients;
using GenAIPlatform.Domain.Observability;

namespace GenAIPlatform.Application.Evaluations.StartRun.Cases;

public interface IEvaluationCostEstimator
{
    Task<CostEstimate?> EstimateAsync(
        AiModelResponse response,
        int? embeddingTokens,
        string? embeddingProvider,
        string? embeddingModel,
        DateTimeOffset usedAtUtc,
        CancellationToken cancellationToken);
}
