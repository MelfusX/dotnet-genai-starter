using GenAIPlatform.Domain.Agentic;

namespace GenAIPlatform.Application.Agentic.Chat;

public sealed record AgentToolCallResult(
    string ToolCallId,
    string ToolName,
    string SchemaVersion,
    string PolicyDecision,
    ToolExecutionStatus ExecutionStatus,
    string? Result,
    string? ErrorCode);
