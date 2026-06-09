namespace GenAIPlatform.Application.Core.ModelClients;

/// <summary>
/// Represents one normalized message in a provider-agnostic chat transcript.
/// </summary>
/// <remarks>
/// Adapters must map this role and content shape to the provider protocol without exposing provider-specific DTOs to application handlers.
/// </remarks>
/// <param name="Role">The provider-agnostic role assigned by the application workflow.</param>
/// <param name="Content">The message text that is safe for the selected model call.</param>
/// <param name="ToolCallId">The provider-normalized tool call identifier when this message is a tool result.</param>
/// <param name="ToolCalls">The tool calls associated with an assistant message when the provider supports tool proposals.</param>
public sealed record AiChatMessage(
    AiMessageRole Role,
    string Content,
    string? ToolCallId = null,
    IReadOnlyList<AiToolCall>? ToolCalls = null);
