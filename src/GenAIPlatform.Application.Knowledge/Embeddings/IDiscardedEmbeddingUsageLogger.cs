namespace GenAIPlatform.Application.Knowledge.Embeddings;

public interface IDiscardedEmbeddingUsageLogger
{
    Task LogDiscardedEmbeddingAsync(
        string correlationId,
        string provider,
        string model,
        int? embeddingTokens,
        TimeSpan latency);
}
