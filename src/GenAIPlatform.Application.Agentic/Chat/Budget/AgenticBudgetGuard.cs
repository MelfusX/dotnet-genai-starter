using GenAIPlatform.Application.Core.ModelClients;

namespace GenAIPlatform.Application.Agentic.Chat;

internal sealed class AgenticBudgetGuard(
    IAgenticCostEstimator costEstimator,
    TimeProvider timeProvider)
{
    public bool IsExceeded(
        int totalTokens,
        decimal estimatedCost,
        AgenticChatOptions options)
    {
        return totalTokens > options.MaxTotalTokens ||
               estimatedCost > options.MaxEstimatedCost;
    }

    public async Task<decimal> EstimateResponseCostAsync(
        AiModelResponse response,
        AgenticChatOptions options,
        CancellationToken cancellationToken)
    {
        try
        {
            var estimate = await costEstimator.EstimateAsync(
                response,
                timeProvider.GetUtcNow(),
                cancellationToken);

            if (estimate is not null)
            {
                return estimate.Value;
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            return EstimateFallbackCost(response.Usage?.TotalTokens, options);
        }

        return EstimateFallbackCost(response.Usage?.TotalTokens, options);
    }

    private static decimal EstimateFallbackCost(
        int? totalTokens,
        AgenticChatOptions options)
    {
        return Math.Round(
            (totalTokens ?? 0) / 1000m * options.EstimatedCostPerThousandTokens,
            8,
            MidpointRounding.AwayFromZero);
    }
}
