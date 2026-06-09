using Microsoft.Extensions.Options;

namespace GenAIPlatform.Application.Generation.Chat;

internal sealed class RagOptionsValidator : IValidateOptions<RagOptions>
{
    public ValidateOptionsResult Validate(string? name, RagOptions options)
    {
        var valid =
            options.DefaultTopK > 0 &&
            options.MaxTopK > 0 &&
            options.DefaultTopK <= options.MaxTopK &&
            options.MaxTopK <= 50 &&
            options.DefaultMinSimilarityScore is >= -1 and <= 1 &&
            options.MaxDocumentFilters is > 0 and <= 100 &&
            options.MaxContextCharacters is >= 500 and <= 64000 &&
            !string.IsNullOrWhiteSpace(options.NoContextFallbackMessage);

        return valid
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail("RAG configuration is invalid.");
    }
}
