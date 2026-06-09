using GenAIPlatform.Application.Core.ModelClients;

namespace GenAIPlatform.Application.Agentic.Chat;

public interface IAgenticCostEstimator
{
    Task<decimal?> EstimateAsync(
        AiModelResponse response,
        DateTimeOffset usedAtUtc,
        CancellationToken cancellationToken);
}
