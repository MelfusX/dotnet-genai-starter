namespace GenAIPlatform.Application.Generation.Chat;

public sealed class RagOptions
{
    public const string SectionName = "GenAIPlatform:Rag";

    public int DefaultTopK { get; init; } = 5;

    public int MaxTopK { get; init; } = 20;

    public double DefaultMinSimilarityScore { get; init; } = 0.2;

    public int MaxDocumentFilters { get; init; } = 50;

    public int MaxContextCharacters { get; init; } = 6000;

    public string NoContextFallbackMessage { get; init; } =
        "I could not find relevant document context for that question.";
}
