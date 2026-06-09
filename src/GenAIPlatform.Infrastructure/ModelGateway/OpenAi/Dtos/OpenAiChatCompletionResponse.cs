using System.Text.Json.Serialization;

namespace GenAIPlatform.Infrastructure.ModelGateway.OpenAi.Dtos;

internal sealed record OpenAiChatCompletionResponse(
    [property: JsonPropertyName("model")] string? Model,
    [property: JsonPropertyName("choices")] IReadOnlyList<OpenAiChoice>? Choices,
    [property: JsonPropertyName("usage")] OpenAiUsage? Usage);
