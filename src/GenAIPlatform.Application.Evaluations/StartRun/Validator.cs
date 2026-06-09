using FluentValidation;
using GenAIPlatform.Application.Generation.Chat;
using Microsoft.Extensions.Options;

namespace GenAIPlatform.Application.Evaluations.StartRun;

internal sealed class StartEvaluationRunValidator : AbstractValidator<StartEvaluationRunCommand>
{
    public StartEvaluationRunValidator(IOptions<RagOptions> ragOptions)
    {
        var maxTopK = Math.Max(1, ragOptions.Value.MaxTopK);

        RuleFor(request => request.TopK)
            .Must(topK =>
            {
                var effectiveTopK = topK ?? ragOptions.Value.DefaultTopK;
                return effectiveTopK >= 1 && effectiveTopK <= maxTopK;
            })
            .WithMessage($"TopK must be between 1 and {maxTopK}.");

        RuleFor(request => request.MinSimilarityScore)
            .Must(score => IsValidSimilarityScore(score ?? ragOptions.Value.DefaultMinSimilarityScore))
            .WithMessage("Minimum similarity score must be between -1 and 1.");
    }

    private static bool IsValidSimilarityScore(double score)
    {
        return !double.IsNaN(score) &&
               !double.IsInfinity(score) &&
               score is >= -1 and <= 1;
    }
}
