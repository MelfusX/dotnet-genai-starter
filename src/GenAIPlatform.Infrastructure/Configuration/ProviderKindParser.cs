namespace GenAIPlatform.Infrastructure.Configuration;

internal static class ProviderKindParser
{
    public static bool TryParse(string? provider, out ProviderKind kind)
    {
        switch (Normalize(provider))
        {
            case "MOCK":
                kind = ProviderKind.Mock;
                return true;
            case "OPENAICOMPATIBLE":
                kind = ProviderKind.OpenAiCompatible;
                return true;
            default:
                kind = default;
                return false;
        }
    }

    public static bool IsOpenAiCompatible(string? provider)
    {
        return TryParse(provider, out var kind) && kind == ProviderKind.OpenAiCompatible;
    }

    private static string Normalize(string? provider)
    {
        return provider?
            .Trim()
            .Replace("_", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal)
            .ToUpperInvariant() ?? string.Empty;
    }

}
