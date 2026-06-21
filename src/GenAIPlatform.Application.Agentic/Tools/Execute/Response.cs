using GenAIPlatform.Domain.Agentic;

namespace GenAIPlatform.Application.Agentic.Tools.Execute;

public sealed record ExecuteToolResponse(
    string ToolCallId,
    string ToolName,
    string SchemaVersion,
    string PolicyDecision,
    ToolExecutionStatus ExecutionStatus,
    string? Result,
    string? ErrorCode,
    string? ErrorMessage);
