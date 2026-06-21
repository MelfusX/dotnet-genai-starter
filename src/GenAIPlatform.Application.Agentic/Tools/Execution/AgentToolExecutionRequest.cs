using System.Text.Json;

namespace GenAIPlatform.Application.Agentic.Tools.Execution;

internal sealed record AgentToolExecutionRequest(
    string ToolCallId,
    string ToolName,
    string? RequestedSchemaVersion,
    JsonElement Arguments,
    IReadOnlyList<IAgentTool> Tools,
    AgentToolExecutionContext Context);
