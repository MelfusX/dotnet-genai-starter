using System.Text.Json.Serialization;

namespace GenAIPlatform.Infrastructure.ModelGateway.OpenAi.Dtos;

internal sealed record OpenAiChoice(
    [property: JsonPropertyName("message")] OpenAiResponseMessage? Message);
