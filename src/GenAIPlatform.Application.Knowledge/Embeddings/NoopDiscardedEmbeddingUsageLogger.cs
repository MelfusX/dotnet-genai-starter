namespace GenAIPlatform.Application.Knowledge.Embeddings;

internal sealed class NoopDiscardedEmbeddingUsageLogger : IDiscardedEmbeddingUsageLogger
{
    public Task LogDiscardedEmbeddingAsync(
        string correlationId,
        string provider,
        string model,
        int? embeddingTokens,
        TimeSpan latency)
    {
        return Task.CompletedTask;
    }
}
