using GenAIPlatform.Application.Core.ModelClients;
using GenAIPlatform.Domain.Observability;

namespace GenAIPlatform.Application.Evaluations.StartRun.Cases;

internal sealed class NoopEvaluationCostEstimator : IEvaluationCostEstimator
{
    public Task<CostEstimate?> EstimateAsync(
        AiModelResponse response,
        int? embeddingTokens,
        string? embeddingProvider,
        string? embeddingModel,
        DateTimeOffset usedAtUtc,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<CostEstimate?>(null);
    }
}
