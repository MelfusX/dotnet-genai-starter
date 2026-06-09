namespace GenAIPlatform.Infrastructure.Configuration;

internal static class OpenAiCompatibleEndpointPolicy
{
    public static bool IsValid(
        string? apiKey,
        string? baseUrl,
        string? endpointPath,
        bool allowInsecureHttpForLoopback,
        int timeoutSeconds,
        int maxRetryAttempts,
        int retryBaseDelayMilliseconds)
    {
        return !string.IsNullOrWhiteSpace(apiKey) &&
               TryCreateEndpointUri(
                   baseUrl,
                   endpointPath,
                   allowInsecureHttpForLoopback,
                   out _) &&
               timeoutSeconds is > 0 and <= 300 &&
               maxRetryAttempts is >= 0 and <= 10 &&
               retryBaseDelayMilliseconds is > 0 and <= 60_000;
    }

    public static bool TryCreateEndpointUri(
        string? baseUrl,
        string? endpointPath,
        bool allowInsecureHttpForLoopback,
        out Uri? endpointUri)
    {
        endpointUri = null;

        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseUri) ||
            !IsAllowedBaseUri(baseUri, allowInsecureHttpForLoopback) ||
            !IsValidEndpointPath(endpointPath))
        {
            return false;
        }

        endpointUri = new Uri(baseUri, endpointPath!);
        return true;
    }

    private static bool IsAllowedBaseUri(
        Uri baseUri,
        bool allowInsecureHttpForLoopback)
    {
        if (baseUri.Scheme == Uri.UriSchemeHttps)
        {
            return true;
        }

        return allowInsecureHttpForLoopback &&
               baseUri.Scheme == Uri.UriSchemeHttp &&
               baseUri.IsLoopback;
    }

    private static bool IsValidEndpointPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) ||
            path.StartsWith("//", StringComparison.Ordinal))
        {
            return false;
        }

        return Uri.TryCreate(path, UriKind.Relative, out _);
    }
}
