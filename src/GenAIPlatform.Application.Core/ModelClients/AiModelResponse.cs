namespace GenAIPlatform.Application.Core.ModelClients;

/// <summary>
/// Carries a normalized chat model response across the application port boundary.
/// </summary>
/// <remarks>
/// Implementations must preserve the requested correlation identifier and must identify the adapter in <see cref="Provider" /> even when the provider omits a provider-specific name.
/// Tool calls are proposals only; backend policy decides whether any proposed call is valid, approved or executable.
/// </remarks>
/// <param name="Content">The assistant content returned by the provider, or an empty string when the provider returned only tool calls.</param>
/// <param name="Model">The provider model name that produced the response.</param>
/// <param name="Provider">The stable adapter identifier that produced the response.</param>
/// <param name="Usage">The provider usage metadata when the provider returned it.</param>
/// <param name="CorrelationId">The application correlation identifier associated with the request.</param>
/// <param name="ProposedToolCalls">The backend-normalized tool calls proposed by the model, or an empty collection when none were proposed.</param>
public sealed record AiModelResponse(
    string Content,
    string Model,
    string Provider,
    AiModelUsage? Usage,
    string CorrelationId,
    IReadOnlyList<AiToolCall>? ProposedToolCalls = null);
