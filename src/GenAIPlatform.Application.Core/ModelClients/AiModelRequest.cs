using GenAIPlatform.Domain.Prompts;

namespace GenAIPlatform.Application.Core.ModelClients;

/// <summary>
/// Carries a rendered chat request from an application use case to a model adapter.
/// </summary>
/// <remarks>
/// Handlers must apply security filtering and prompt rendering before creating this request; adapters must treat the message list as already authorized application input.
/// </remarks>
/// <param name="CorrelationId">The application correlation identifier that adapters must pass through to provider metadata when supported.</param>
/// <param name="Model">The resolved provider model name to use for the call.</param>
/// <param name="Messages">The ordered chat transcript that will be sent to the provider.</param>
/// <param name="Temperature">The optional sampling temperature after application-level policy resolution.</param>
/// <param name="MaxOutputTokens">The optional provider output-token limit after application-level policy resolution.</param>
/// <param name="Prompt">The optional prompt template metadata associated with the rendered request.</param>
/// <param name="Tools">The optional backend-owned tool definitions that may be offered to the model.</param>
public sealed record AiModelRequest(
    string CorrelationId,
    string Model,
    IReadOnlyList<AiChatMessage> Messages,
    double? Temperature = null,
    int? MaxOutputTokens = null,
    PromptMetadata? Prompt = null,
    IReadOnlyList<AiToolDefinition>? Tools = null);
