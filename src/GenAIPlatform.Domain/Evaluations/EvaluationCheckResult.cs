namespace GenAIPlatform.Domain.Evaluations;

public sealed record EvaluationCheckResult(
    string Type,
    bool Passed,
    string Message);
