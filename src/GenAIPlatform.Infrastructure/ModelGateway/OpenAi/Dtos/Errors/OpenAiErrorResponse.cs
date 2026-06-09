using System.Text.Json.Serialization;

namespace GenAIPlatform.Infrastructure.ModelGateway.OpenAi.Dtos;

internal sealed record OpenAiErrorResponse(
    [property: JsonPropertyName("error")] OpenAiError? Error);
