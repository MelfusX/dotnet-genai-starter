using System.Text.Json.Serialization;

namespace GenAIPlatform.Infrastructure.ModelGateway.OpenAi.Dtos;

internal sealed record OpenAiChatMessage(
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("content")] string Content,
    [property: JsonPropertyName("tool_call_id")] string? ToolCallId,
    [property: JsonPropertyName("tool_calls")] IReadOnlyList<OpenAiToolCall>? ToolCalls);
