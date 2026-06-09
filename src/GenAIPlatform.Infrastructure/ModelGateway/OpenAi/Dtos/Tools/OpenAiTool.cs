using System.Text.Json.Serialization;

namespace GenAIPlatform.Infrastructure.ModelGateway.OpenAi.Dtos;

internal sealed record OpenAiTool(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("function")] OpenAiToolFunction Function);
