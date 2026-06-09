using System.Text.Json.Serialization;

namespace GenAIPlatform.Infrastructure.ModelGateway.OpenAi.Dtos;

internal sealed record OpenAiResponseMessage(
    [property: JsonPropertyName("content")] string? Content,
    [property: JsonPropertyName("tool_calls")] IReadOnlyList<OpenAiToolCall>? ToolCalls);
