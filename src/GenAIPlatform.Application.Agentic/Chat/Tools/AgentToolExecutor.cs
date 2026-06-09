using GenAIPlatform.Application.Agentic.Validation;
using GenAIPlatform.Application.Agentic.Tools;
using GenAIPlatform.Domain.Agentic;
using System.Text.Json;
using GenAIPlatform.Application.Core.ModelClients;
using Microsoft.Extensions.Logging;

namespace GenAIPlatform.Application.Agentic.Chat;

internal sealed class AgentToolExecutor(
    ToolPolicy toolPolicy,
    AgentToolAuditWriter auditWriter,
    ILogger<AgentToolExecutor> logger)
{
    public async Task<AgentToolCallResult> ExecuteAsync(
        AgenticChatSession session,
        AiToolCall toolCall,
        CancellationToken cancellationToken)
    {
        var tool = session.Tools.FirstOrDefault(candidate =>
            string.Equals(candidate.Definition.Name, toolCall.Name, StringComparison.Ordinal));
        var validation = tool?.Validate(toolCall.Arguments)
            ?? ToolValidationResult.Invalid("unknown_tool", "The requested tool is not available.");
        var policy = toolPolicy.Decide(tool?.Policy, toolCall.Name);
        var execution = await ExecuteCoreAsync(
            session,
            toolCall,
            tool,
            validation,
            policy,
            cancellationToken);

        await auditWriter.AuditToolCallAsync(
            session,
            toolCall,
            validation,
            policy,
            execution.ApprovalState,
            execution.ExecutionStatus,
            execution.Output,
            execution.ErrorCode,
            execution.ErrorMessage);

        return new AgentToolCallResult(
            toolCall.Id,
            toolCall.Name,
            AgentToolSchemaVersion.Resolve(toolCall.SchemaVersion),
            policy.Decision,
            execution.ExecutionStatus,
            execution.ResultText,
            execution.ErrorCode);
    }

    private async Task<AgentToolExecutionOutcome> ExecuteCoreAsync(
        AgenticChatSession session,
        AiToolCall toolCall,
        IAgentTool? tool,
        ToolValidationResult validation,
        ToolPolicyDecision policy,
        CancellationToken cancellationToken)
    {
        if (policy.Risk == ToolRisk.Forbidden)
        {
            return AgentToolExecutionOutcome.Rejected("tool_forbidden", policy.Reason);
        }

        if (!validation.IsValid)
        {
            return AgentToolExecutionOutcome.ValidationFailed(
                validation.ErrorCode,
                validation.ErrorMessage);
        }

        if (policy.RequiresApproval && !session.ApproveRiskyTools)
        {
            return AgentToolExecutionOutcome.ApprovalRequired(policy.Reason);
        }

        if (!policy.MayExecute)
        {
            return AgentToolExecutionOutcome.Rejected("tool_not_executable", policy.Reason);
        }

        return tool is null
            ? AgentToolExecutionOutcome.Rejected("unknown_tool", "The requested tool is not available.")
            : await ExecuteBackendToolAsync(
                session,
                toolCall,
                tool,
                validation.SanitizedArguments,
                policy,
                cancellationToken);
    }

    private async Task<AgentToolExecutionOutcome> ExecuteBackendToolAsync(
        AgenticChatSession session,
        AiToolCall toolCall,
        IAgentTool tool,
        JsonElement sanitizedArguments,
        ToolPolicyDecision policy,
        CancellationToken cancellationToken)
    {
        try
        {
            var execution = await tool.ExecuteAsync(
                sanitizedArguments,
                cancellationToken);
            if (!IsBackendExecutionStatus(execution.Status))
            {
                return AgentToolExecutionOutcome.UnexpectedBackendStatus(
                    policy.RequiresApproval,
                    execution.Status);
            }

            return AgentToolExecutionOutcome.Executed(
                policy.RequiresApproval,
                execution.Status,
                execution.Output,
                execution.ErrorCode,
                execution.ErrorMessage);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            return AgentToolExecutionOutcome.Failed(
                "tool_execution_canceled",
                "Tool execution was canceled.");
        }
        catch (Exception exception)
        {
            logger.LogError(
                exception,
                "Agent tool execution failed. ToolName={ToolName} ToolCallId={ToolCallId} ConversationId={ConversationId} CorrelationId={CorrelationId}",
                tool.Definition.Name,
                toolCall.Id,
                session.ConversationId,
                session.Settings.CorrelationId);

            return AgentToolExecutionOutcome.Failed(
                "tool_execution_failed",
                "Tool execution failed.");
        }
    }

    private static bool IsBackendExecutionStatus(ToolExecutionStatus status)
    {
        return status is ToolExecutionStatus.Succeeded or ToolExecutionStatus.Failed;
    }
}
