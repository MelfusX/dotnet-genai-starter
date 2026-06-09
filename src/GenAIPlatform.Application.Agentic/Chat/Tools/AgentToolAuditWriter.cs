using GenAIPlatform.Application.Agentic.Validation;
using GenAIPlatform.Application.Agentic.Tools;
using GenAIPlatform.Domain.Agentic;
using System.Text.Json;
using GenAIPlatform.Application.Core.ModelClients;

namespace GenAIPlatform.Application.Agentic.Chat;

internal sealed class AgentToolAuditWriter(
    IToolAuditLogRepository auditLogRepository,
    ToolPolicy toolPolicy,
    TimeProvider timeProvider)
{
    public async Task AuditSkippedToolCallsAsync(
        AgenticChatSession session,
        IReadOnlyList<AiToolCall> toolCalls,
        ToolExecutionStatus executionStatus,
        string errorCode,
        string errorMessage)
    {
        foreach (var toolCall in toolCalls)
        {
            var tool = FindTool(
                session.Tools,
                toolCall.Name);
            var validation = tool?.Validate(toolCall.Arguments)
                ?? ToolValidationResult.Invalid("unknown_tool", "The requested tool is not available.");
            var policy = toolPolicy.Decide(tool?.Policy, toolCall.Name);

            await AuditToolCallAsync(
                session,
                toolCall,
                validation,
                policy,
                ToolApprovalState.NotRequired,
                executionStatus,
                output: null,
                errorCode,
                errorMessage);
        }
    }

    public async Task AuditToolCallAsync(
        AgenticChatSession session,
        AiToolCall toolCall,
        ToolValidationResult validation,
        ToolPolicyDecision policy,
        ToolApprovalState approvalState,
        ToolExecutionStatus executionStatus,
        JsonElement? output,
        string? errorCode,
        string? errorMessage)
    {
        await auditLogRepository.AddAsync(
            new ToolAuditLogEntry(
                Guid.NewGuid(),
                session.ConversationId,
                session.TenantId,
                session.UserId,
                session.Settings.CorrelationId,
                toolCall.Id,
                toolCall.Name,
                AgentToolSchemaVersion.Resolve(toolCall.SchemaVersion),
                session.Options.PolicyVersion,
                validation.Status.ToPublicValue(),
                policy.Decision,
                approvalState.ToPublicValue(),
                executionStatus.ToPublicValue(),
                validation.SanitizedArguments,
                output,
                errorCode,
                errorMessage,
                timeProvider.GetUtcNow()),
            CancellationToken.None);
    }

    private static IAgentTool? FindTool(
        IReadOnlyList<IAgentTool> tools,
        string toolName)
    {
        return tools.FirstOrDefault(candidate =>
            string.Equals(candidate.Definition.Name, toolName, StringComparison.Ordinal));
    }
}
