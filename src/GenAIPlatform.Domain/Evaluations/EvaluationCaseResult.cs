namespace GenAIPlatform.Domain.Evaluations;

public sealed record EvaluationCaseResult(
    string CaseId,
    string Name,
    string Status,
    string? Answer,
    int RetrievedCount,
    bool RetrievalHit,
    TimeSpan Latency,
    decimal EstimatedCost,
    string CostCurrency,
    string? ErrorCode,
    string? ErrorMessage,
    IReadOnlyList<EvaluationCheckResult> Checks);
