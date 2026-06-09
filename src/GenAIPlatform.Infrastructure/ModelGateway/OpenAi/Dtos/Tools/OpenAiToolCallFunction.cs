using System.Text.Json.Serialization;

namespace GenAIPlatform.Infrastructure.ModelGateway.OpenAi.Dtos;

internal sealed record OpenAiToolCallFunction(
    [property: JsonPropertyName("name")] string? Name,
    [property: JsonPropertyName("arguments")] string? Arguments);
