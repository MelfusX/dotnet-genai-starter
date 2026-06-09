namespace GenAIPlatform.Application.Generation.Prompts.Rendering;

/// <summary>
/// Defines the application service that renders active prompt templates.
/// </summary>
/// <remarks>
/// Implementations must resolve the active template version through application-owned prompt metadata and must return enough metadata for downstream logging without logging the rendered prompt by default.
/// Missing templates or unresolved variables must fail before provider calls are made.
/// </remarks>
public interface IPromptRenderer
{
    /// <summary>
    /// Renders the active version of a prompt template with validated variables.
    /// </summary>
    /// <remarks>
    /// Implementations must preserve template metadata in the returned <see cref="RenderedPrompt" /> so model responses can cite the prompt version and content hash.
    /// </remarks>
    Task<RenderedPrompt> RenderActiveAsync(
        string templateName,
        IReadOnlyDictionary<string, string> variables,
        CancellationToken cancellationToken);
}
