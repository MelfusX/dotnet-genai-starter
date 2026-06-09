namespace GenAIPlatform.Domain.Evaluations;

public sealed record EvaluationCheck(
    string Type,
    string? Phrase = null,
    int? MinimumHits = null,
    IReadOnlyList<string>? CitationReferences = null);
