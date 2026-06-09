using System.Text.Json;

namespace GenAIPlatform.Application.Core.ModelClients;

/// <summary>
/// Represents a provider-normalized tool call proposed by a model.
/// </summary>
/// <remarks>
/// Tool calls must be treated as untrusted proposals until backend validation, policy and approval checks have run.
/// </remarks>
/// <param name="Id">The provider-supplied or adapter-generated identifier for correlating tool results.</param>
/// <param name="Name">The backend tool name proposed by the model.</param>
/// <param name="SchemaVersion">The backend schema version associated with the proposed tool name.</param>
/// <param name="Arguments">The raw JSON argument payload normalized to a <see cref="JsonElement" /> for backend validation.</param>
public sealed record AiToolCall(
    string Id,
    string Name,
    string SchemaVersion,
    JsonElement Arguments);
