namespace GenAIPlatform.Application.Generation.Chat;

public sealed record RagPromptContext(
    string ContextText,
    IReadOnlyList<RagCitation> Citations);
