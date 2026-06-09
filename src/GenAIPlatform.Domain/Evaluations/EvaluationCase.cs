namespace GenAIPlatform.Domain.Evaluations;

public sealed record EvaluationCase(
    string Id,
    string Name,
    string Question,
    IReadOnlyList<EvaluationCheck> Checks,
    string? Context = null);
