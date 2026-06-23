using GenAIPlatform.Domain.Agentic;

namespace GenAIPlatform.Application.Agentic.Tools.Execution;

internal sealed class AgentToolAuditLogWriter(
    IToolAuditLogRepository auditLogRepository,
    TimeProvider timeProvider)
{
    public Task WriteAsync(
        AgentToolExecutionResult result,
        AgentToolExecutionContext context,
        CancellationToken cancellationToken)
    {
        return auditLogRepository.AddAsync(
            new ToolAuditLogEntry(
                Guid.NewGuid(),
                context.ConversationId,
                context.TenantId,
                context.UserId,
                context.CorrelationId,
                result.ToolCallId,
                result.ToolName,
                result.AuditSchemaVersion,
                context.PolicyVersion,
                result.Validation.Status.ToPublicValue(),
                result.Policy.Decision,
                result.ApprovalState.ToPublicValue(),
                result.ExecutionStatus.ToPublicValue(),
                result.Validation.SanitizedArguments,
                result.Output,
                result.ErrorCode,
                result.ErrorMessage,
                timeProvider.GetUtcNow()),
            cancellationToken);
    }
}
