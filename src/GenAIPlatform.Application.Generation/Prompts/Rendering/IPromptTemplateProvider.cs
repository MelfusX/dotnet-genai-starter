using GenAIPlatform.Domain.Prompts;

namespace GenAIPlatform.Application.Generation.Prompts.Rendering;

/// <summary>
/// Defines the application-owned source for active prompt template versions.
/// </summary>
/// <remarks>
/// Implementations must return only active prompt versions and must not resolve provider-specific prompt behavior.
/// Returning <see langword="null" /> means no active template is available for the supplied name.
/// </remarks>
public interface IPromptTemplateProvider
{
    /// <summary>
    /// Gets the active version for a prompt template name.
    /// </summary>
    /// <remarks>
    /// Implementations must honor cancellation before doing durable storage or network work and must preserve template content hashes exactly as stored.
    /// </remarks>
    Task<PromptTemplateVersion?> GetActiveVersionAsync(
        string templateName,
        CancellationToken cancellationToken);
}
