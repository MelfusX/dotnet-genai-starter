using GenAIPlatform.Application.Core.Embeddings;

namespace GenAIPlatform.Application.Knowledge.Embeddings;

public static class EmbeddingVectorValidator
{
    public const string EmptyEmbeddingErrorCode = "empty_embedding";
    public const string InvalidEmbeddingErrorCode = "invalid_embedding";
    private const string UnknownProviderName = "unknown";

    public static void EnsureValidCosineVector(EmbeddingResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        var provider = GetSafeProviderName(response.Provider);
        if (response.Vector is null)
        {
            throw new EmbeddingClientException(
                provider,
                "Embedding provider returned an invalid vector.",
                errorCode: InvalidEmbeddingErrorCode);
        }

        if (string.IsNullOrWhiteSpace(response.Provider))
        {
            throw new EmbeddingClientException(
                provider,
                "Embedding provider returned blank provider metadata.",
                errorCode: InvalidEmbeddingErrorCode);
        }

        if (string.IsNullOrWhiteSpace(response.Model))
        {
            throw new EmbeddingClientException(
                provider,
                "Embedding provider returned blank model metadata.",
                errorCode: InvalidEmbeddingErrorCode);
        }

        if (response.InputTokens is < 0)
        {
            throw new EmbeddingClientException(
                provider,
                "Embedding provider returned invalid token metadata.",
                errorCode: InvalidEmbeddingErrorCode);
        }

        if (response.Vector.Count == 0)
        {
            throw new EmbeddingClientException(
                provider,
                "Embedding provider returned an empty vector.",
                errorCode: EmptyEmbeddingErrorCode);
        }

        var hasNonZeroValue = false;
        foreach (var value in response.Vector)
        {
            if (!float.IsFinite(value))
            {
                throw new EmbeddingClientException(
                    provider,
                    "Embedding provider returned a vector with non-finite values.",
                    errorCode: InvalidEmbeddingErrorCode);
            }

            hasNonZeroValue |= value != 0f;
        }

        if (!hasNonZeroValue)
        {
            throw new EmbeddingClientException(
                provider,
                "Embedding provider returned a zero vector.",
                errorCode: InvalidEmbeddingErrorCode);
        }
    }

    private static string GetSafeProviderName(string? provider)
    {
        return string.IsNullOrWhiteSpace(provider)
            ? UnknownProviderName
            : provider.Trim();
    }
}
