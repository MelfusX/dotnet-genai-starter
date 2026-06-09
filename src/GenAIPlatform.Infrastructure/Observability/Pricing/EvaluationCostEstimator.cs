using GenAIPlatform.Application.Core.ModelClients;
using GenAIPlatform.Application.Evaluations.StartRun.Cases;
using GenAIPlatform.Domain.Observability;

namespace GenAIPlatform.Infrastructure.Observability.Pricing;

internal sealed class EvaluationCostEstimator(AiCostEstimator costEstimator) : IEvaluationCostEstimator
{
    public Task<CostEstimate?> EstimateAsync(
        AiModelResponse response,
        int? embeddingTokens,
        string? embeddingProvider,
        string? embeddingModel,
        DateTimeOffset usedAtUtc,
        CancellationToken cancellationToken)
    {
        return costEstimator.EstimateAsync(
            response.Provider,
            response.Model,
            response.Usage,
            embeddingTokens,
            embeddingProvider,
            embeddingModel,
            usedAtUtc,
            cancellationToken);
    }
}
