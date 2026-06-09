using GenAIPlatform.Application.Core.ModelClients;

namespace GenAIPlatform.Application.Agentic.Chat;

internal sealed class NoopAgenticCostEstimator : IAgenticCostEstimator
{
    public Task<decimal?> EstimateAsync(
        AiModelResponse response,
        DateTimeOffset usedAtUtc,
        CancellationToken cancellationToken)
    {
        return Task.FromResult<decimal?>(null);
    }
}
