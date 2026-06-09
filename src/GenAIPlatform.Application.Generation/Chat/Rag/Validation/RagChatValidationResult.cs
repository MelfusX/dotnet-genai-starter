namespace GenAIPlatform.Application.Generation.Chat;

public sealed record RagChatValidationResult(
    string Message,
    int TopK,
    double MinSimilarityScore,
    IReadOnlyCollection<Guid> DocumentIds);
