using GenAIPlatform.Application.Evaluations.StartRun;
using GenAIPlatform.Domain.Evaluations;

namespace GenAIPlatform.Application.Evaluations;

public interface IEvaluationRunRepository
{
    Task AddRunAsync(
        EvaluationRunResult run,
        string tenantId,
        string userId,
        CancellationToken cancellationToken);

    Task AddCaseResultAsync(Guid runId, EvaluationCaseResult result, CancellationToken cancellationToken);

    Task CompleteRunAsync(
        Guid runId,
        string status,
        DateTimeOffset completedAtUtc,
        CancellationToken cancellationToken);

    Task<EvaluationRunResult?> GetRunAsync(
        Guid runId,
        string tenantId,
        string userId,
        CancellationToken cancellationToken);

    Task<EvaluationRunSummary?> GetSummaryAsync(
        Guid runId,
        string tenantId,
        string userId,
        CancellationToken cancellationToken);
}
