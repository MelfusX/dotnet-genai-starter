namespace GenAIPlatform.Domain.Prompts;

public sealed record PromptTemplateSeed(
    string? TemplateName,
    string? Version,
    PromptTemplateStatus Status,
    string? SystemMessage,
    string? UserMessageTemplate,
    IReadOnlyCollection<string>? Variables,
    DateTimeOffset CreatedAtUtc,
    string? Description);
