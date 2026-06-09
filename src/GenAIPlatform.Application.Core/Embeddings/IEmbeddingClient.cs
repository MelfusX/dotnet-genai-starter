namespace GenAIPlatform.Application.Core.Embeddings;

/// <summary>
/// Defines the application-owned port for embedding provider adapters.
/// </summary>
/// <remarks>
/// Implementations must be safe to use from multiple concurrent request scopes and must not retain mutable request state between calls.
/// Provider SDK, HTTP and serialization failures must be normalized to application-level provider exceptions before crossing this boundary.
/// Implementations must not log input text, provider credentials, raw embedding vectors or full provider responses.
/// </remarks>
public interface IEmbeddingClient
{
    /// <summary>
    /// Creates an embedding for one input.
    /// </summary>
    /// <remarks>
    /// Implementations must honor cancellation when possible because worker lease ownership can change while a provider call is in flight.
    /// Throw <see cref="OperationCanceledException" /> only when the caller-provided token is canceled.
    /// Provider-side timeouts or internal cancellations must be normalized as provider failures so indexing attempt accounting can be deterministic.
    /// </remarks>
    Task<EmbeddingResponse> CreateEmbeddingAsync(
        EmbeddingRequest request,
        CancellationToken cancellationToken);
}
