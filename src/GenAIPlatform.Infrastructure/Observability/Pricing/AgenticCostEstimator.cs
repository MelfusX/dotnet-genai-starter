using GenAIPlatform.Application.Agentic.Chat;
using GenAIPlatform.Application.Core.ModelClients;

namespace GenAIPlatform.Infrastructure.Observability.Pricing;

internal sealed class AgenticCostEstimator(AiCostEstimator costEstimator) : IAgenticCostEstimator
{
    public async Task<decimal?> EstimateAsync(
        AiModelResponse response,
        DateTimeOffset usedAtUtc,
        CancellationToken cancellationToken)
    {
        var estimate = await costEstimator.EstimateAsync(
            response.Provider,
            response.Model,
            response.Usage,
            embeddingTokens: null,
            embeddingProvider: null,
            embeddingModel: null,
            usedAtUtc,
            cancellationToken);

        return estimate?.Amount;
    }
}
