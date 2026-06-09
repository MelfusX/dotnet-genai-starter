using System.Text.Json;

namespace GenAIPlatform.Domain.Agentic;

public sealed record ToolAuditLogEntry(
    Guid Id,
    Guid ConversationId,
    string TenantId,
    string UserId,
    string CorrelationId,
    string ToolCallId,
    string ToolName,
    string SchemaVersion,
    string PolicyVersion,
    string ValidationStatus,
    string PolicyDecision,
    string ApprovalState,
    string ExecutionStatus,
    JsonElement Arguments,
    JsonElement? Output,
    string? ErrorCode,
    string? ErrorMessage,
    DateTimeOffset CreatedAtUtc);
