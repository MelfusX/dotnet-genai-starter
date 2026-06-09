namespace GenAIPlatform.Domain.Evaluations;

public sealed record EvaluationFailedCase(
    string CaseId,
    string Name,
    string Status,
    string? ErrorCode,
    IReadOnlyList<EvaluationCheckResult> Checks);
