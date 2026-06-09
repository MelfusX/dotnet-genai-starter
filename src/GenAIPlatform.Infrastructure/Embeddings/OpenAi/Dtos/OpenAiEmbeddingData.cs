using System.Text.Json.Serialization;

namespace GenAIPlatform.Infrastructure.Embeddings.OpenAi.Dtos;

internal sealed record OpenAiEmbeddingData(
    [property: JsonPropertyName("embedding")] IReadOnlyList<float>? Embedding);
