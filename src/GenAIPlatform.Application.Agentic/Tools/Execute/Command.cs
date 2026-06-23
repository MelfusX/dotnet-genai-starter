using GenAIPlatform.Application.Core.Dispatching;
using System.Text.Json;

namespace GenAIPlatform.Application.Agentic.Tools.Execute;

public sealed record ExecuteToolCommand(
    string? ToolName = null,
    JsonElement Arguments = default,
    string? SchemaVersion = null)
    : IRequest<ExecuteToolResponse>;
