namespace GenAIPlatform.Domain.Prompts;

public sealed record PromptTemplateVersion(
    string TemplateName,
    string Version,
    PromptTemplateStatus Status,
    string ContentHash,
    string SystemMessage,
    string UserMessageTemplate,
    IReadOnlyCollection<string> Variables,
    DateTimeOffset CreatedAtUtc,
    string? Description)
{
    public static PromptTemplateVersion Create(
        string templateName,
        string version,
        PromptTemplateStatus status,
        string systemMessage,
        string userMessageTemplate,
        IReadOnlyCollection<string> variables,
        DateTimeOffset createdAtUtc,
        string? description = null)
    {
        return new PromptTemplateVersion(
            templateName,
            version,
            status,
            PromptContentHasher.Compute(systemMessage, userMessageTemplate),
            systemMessage,
            userMessageTemplate,
            variables.ToArray(),
            createdAtUtc,
            description);
    }
}
