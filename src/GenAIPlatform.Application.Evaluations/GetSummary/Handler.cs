using GenAIPlatform.Application.Core.Dispatching;
using GenAIPlatform.Application.Core.Security;

namespace GenAIPlatform.Application.Evaluations;

public sealed class GetEvaluationSummaryHandler(
    IEvaluationRunRepository repository,
    IUserContext userContext)
    : IRequestHandler<GetEvaluationSummaryQuery, EvaluationRunSummary?>
{
    public Task<EvaluationRunSummary?> HandleAsync(
        GetEvaluationSummaryQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = userContext.TenantId;
        var userId = userContext.UserId;
        if (!userContext.IsAuthenticated ||
            string.IsNullOrWhiteSpace(tenantId) ||
            string.IsNullOrWhiteSpace(userId))
        {
            return Task.FromResult<EvaluationRunSummary?>(null);
        }

        return repository.GetSummaryAsync(request.RunId, tenantId, userId, cancellationToken);
    }
}
