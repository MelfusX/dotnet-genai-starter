using GenAIPlatform.Application.Core.Security;
using GenAIPlatform.Application.Generation.Chat;
using Microsoft.Extensions.Options;

namespace GenAIPlatform.Application.Evaluations.StartRun;

internal sealed class StartEvaluationRunNormalizer(
    IUserContext userContext,
    IOptions<RagOptions> ragOptions)
{
    public StartEvaluationRunValidationResult Normalize(StartEvaluationRunCommand request)
    {
        return new StartEvaluationRunValidationResult(
            userContext.RequireAuthenticatedTenant(),
            userContext.RequireAuthenticatedUser(),
            request.TopK ?? ragOptions.Value.DefaultTopK,
            request.MinSimilarityScore ?? ragOptions.Value.DefaultMinSimilarityScore);
    }
}
