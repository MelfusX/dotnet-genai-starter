namespace GenAIPlatform.Application.Core.ModelClients;

/// <summary>
/// Defines provider-agnostic roles used in chat transcripts.
/// </summary>
public enum AiMessageRole
{
    /// <summary>
    /// Identifies application-controlled instructions that guide the model response.
    /// </summary>
    System = 0,

    /// <summary>
    /// Identifies foreground user content after application validation and authorization.
    /// </summary>
    User = 1,

    /// <summary>
    /// Identifies assistant output or assistant tool-call proposals returned by a model.
    /// </summary>
    Assistant = 2,

    /// <summary>
    /// Identifies backend-produced tool execution results returned to the model loop.
    /// </summary>
    Tool = 3
}
