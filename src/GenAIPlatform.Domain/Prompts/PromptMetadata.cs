namespace GenAIPlatform.Domain.Prompts;

public sealed record PromptMetadata(
    string TemplateName,
    string Version,
    string ContentHash);
