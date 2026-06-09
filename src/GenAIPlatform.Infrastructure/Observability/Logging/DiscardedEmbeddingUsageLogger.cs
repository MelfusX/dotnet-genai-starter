using GenAIPlatform.Application.Knowledge.Embeddings;

namespace GenAIPlatform.Infrastructure.Observability.Logging;

internal sealed class DiscardedEmbeddingUsageLogger(
    AiModelRequestLoggingService requestLoggingService)
    : IDiscardedEmbeddingUsageLogger
{
    public Task LogDiscardedEmbeddingAsync(
        string correlationId,
        string provider,
        string model,
        int? embeddingTokens,
        TimeSpan latency)
    {
        return requestLoggingService.LogDiscardedEmbeddingAsync(
            correlationId,
            provider,
            model,
            embeddingTokens,
            latency);
    }
}
