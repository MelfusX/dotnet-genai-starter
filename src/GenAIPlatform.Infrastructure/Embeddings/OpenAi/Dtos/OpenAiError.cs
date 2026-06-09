using System.Text.Json.Serialization;

namespace GenAIPlatform.Infrastructure.Embeddings.OpenAi.Dtos;

internal sealed record OpenAiError(
    [property: JsonPropertyName("message")] string? Message,
    [property: JsonPropertyName("code")] string? Code);
