using System.Text.Json.Serialization;

namespace GenAIPlatform.Infrastructure.Embeddings.OpenAi.Dtos;

internal sealed record OpenAiErrorResponse(
    [property: JsonPropertyName("error")] OpenAiError? Error);
