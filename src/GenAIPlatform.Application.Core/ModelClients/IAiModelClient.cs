namespace GenAIPlatform.Application.Core.ModelClients;

/// <summary>
/// Defines the application-owned port for chat model completion adapters.
/// </summary>
/// <remarks>
/// Implementations must be safe to use from multiple concurrent request scopes and must not retain mutable request state between calls.
/// Provider SDK, HTTP and serialization failures must be normalized to application-level provider exceptions before crossing this boundary.
/// Implementations must throw <see cref="OperationCanceledException" /> only when the supplied cancellation token is canceled; provider timeouts and internal aborts must be reported as provider failures so retry and usage accounting stay deterministic.
/// The returned <see cref="AiModelResponse.Provider" /> value identifies the adapter that produced the response, and <see cref="AiModelResponse.ProposedToolCalls" /> must be empty when the provider did not propose tool calls.
/// </remarks>
public interface IAiModelClient
{
    /// <summary>
    /// Completes a chat request through the configured model provider.
    /// </summary>
    /// <remarks>
    /// Implementations must not log rendered prompt text, tool arguments containing user data, provider credentials or raw provider responses unless a separate redaction and opt-in policy owns that logging.
    /// If a provider returns usage metadata, implementations must preserve it in the response without inventing token counts.
    /// </remarks>
    Task<AiModelResponse> CompleteAsync(
        AiModelRequest request,
        CancellationToken cancellationToken);
}
