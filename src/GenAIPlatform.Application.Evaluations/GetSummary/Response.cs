using GenAIPlatform.Domain.Evaluations;

namespace GenAIPlatform.Application.Evaluations;

public sealed record EvaluationRunSummary(
    Guid RunId,
    string DatasetVersion,
    string Status,
    int TotalCases,
    int PassedCases,
    int FailedCaseCount,
    double RetrievalHitRate,
    double AverageLatencyMs,
    decimal AverageCost,
    string CostCurrency,
    IReadOnlyList<EvaluationFailedCase> FailedCases);
