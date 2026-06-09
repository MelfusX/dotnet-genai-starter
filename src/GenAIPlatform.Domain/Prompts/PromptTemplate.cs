namespace GenAIPlatform.Domain.Prompts;

public sealed record PromptTemplate(
    string Name,
    IReadOnlyCollection<PromptTemplateVersion> Versions);
