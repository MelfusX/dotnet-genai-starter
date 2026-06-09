using System.Text.Json.Serialization;

namespace GenAIPlatform.Infrastructure.ModelGateway.OpenAi.Dtos;

internal sealed record OpenAiUsage(
    [property: JsonPropertyName("prompt_tokens")] int? PromptTokens,
    [property: JsonPropertyName("completion_tokens")] int? CompletionTokens,
    [property: JsonPropertyName("total_tokens")] int? TotalTokens);
