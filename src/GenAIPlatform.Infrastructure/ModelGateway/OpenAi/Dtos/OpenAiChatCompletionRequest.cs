using System.Text.Json.Serialization;

namespace GenAIPlatform.Infrastructure.ModelGateway.OpenAi.Dtos;

internal sealed record OpenAiChatCompletionRequest(
    [property: JsonPropertyName("model")] string Model,
    [property: JsonPropertyName("messages")] IReadOnlyList<OpenAiChatMessage> Messages,
    [property: JsonPropertyName("temperature")] double? Temperature,
    [property: JsonPropertyName("max_tokens")] int? MaxTokens,
    [property: JsonPropertyName("tools")] IReadOnlyList<OpenAiTool>? Tools);
