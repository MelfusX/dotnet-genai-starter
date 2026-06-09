using System.Text.Json;

namespace GenAIPlatform.Application.Core.ModelClients;

/// <summary>
/// Describes a backend-owned tool that may be offered to a model.
/// </summary>
/// <remarks>
/// The definition is model-facing metadata only; executable behavior, risk, approval and audit policy remain owned by backend tool implementations.
/// </remarks>
/// <param name="Name">The stable backend tool name the model may reference in a proposed tool call.</param>
/// <param name="Description">The concise model-facing description of the tool's intended use.</param>
/// <param name="SchemaVersion">The backend schema version for the tool argument contract.</param>
/// <param name="InputSchema">The JSON schema that describes valid tool arguments for model planning.</param>
public sealed record AiToolDefinition(
    string Name,
    string Description,
    string SchemaVersion,
    JsonElement InputSchema);
