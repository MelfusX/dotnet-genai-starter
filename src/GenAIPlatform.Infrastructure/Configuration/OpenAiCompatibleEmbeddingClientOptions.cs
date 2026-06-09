namespace GenAIPlatform.Infrastructure.Configuration;

public sealed class OpenAiCompatibleEmbeddingClientOptions
{
    public const string SectionName = "GenAIPlatform:Embeddings:OpenAiCompatible";

    public string BaseUrl { get; init; } = "https://api.openai.com";

    public string EmbeddingsPath { get; init; } = "/v1/embeddings";

    public string? ApiKey { get; init; }

    public string? Organization { get; init; }

    public int TimeoutSeconds { get; init; } = 30;

    public int MaxRetryAttempts { get; init; } = 2;

    public int RetryBaseDelayMilliseconds { get; init; } = 200;

    public bool AllowInsecureHttpForLoopback { get; init; }

    public bool IsValid()
    {
        return OpenAiCompatibleEndpointPolicy.IsValid(
            ApiKey,
            BaseUrl,
            EmbeddingsPath,
            AllowInsecureHttpForLoopback,
            TimeoutSeconds,
            MaxRetryAttempts,
            RetryBaseDelayMilliseconds);
    }

    public bool TryCreateEndpointUri(out Uri? endpointUri)
    {
        return OpenAiCompatibleEndpointPolicy.TryCreateEndpointUri(
            BaseUrl,
            EmbeddingsPath,
            AllowInsecureHttpForLoopback,
            out endpointUri);
    }
}
