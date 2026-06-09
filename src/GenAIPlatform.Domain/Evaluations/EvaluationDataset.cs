namespace GenAIPlatform.Domain.Evaluations;

public sealed record EvaluationDataset(
    string Version,
    IReadOnlyList<EvaluationCase> Cases);
