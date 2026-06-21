using GenAIPlatform.Application.Agentic.Tools.Execution;
using GenAIPlatform.Application.Core.ModelClients;
using Microsoft.Extensions.Logging;

namespace GenAIPlatform.Application.Agentic.Chat;

internal sealed class AgentToolExecutor(
    GovernedAgentToolExecutor toolExecutor,
    ILogger<AgentToolExecutor> logger)
{
    public async Task<AgentToolCallResult> ExecuteAsync(
        AgenticChatSession session,
        AiToolCall toolCall,
        CancellationToken cancellationToken)
    {
        var execution = await toolExecutor.ExecuteAsync(
            new AgentToolExecutionRequest(
                toolCall.Id,
                toolCall.Name,
                toolCall.SchemaVersion,
                toolCall.Arguments,
                session.Tools,
                new AgentToolExecutionContext(
                    session.ConversationId,
                    session.TenantId,
                    session.UserId,
                    session.Settings.CorrelationId,
                    session.Options.PolicyVersion,
                    session.ApproveRiskyTools)),
            cancellationToken);

        LogExecutionFailure(
            execution,
            session);

        return new AgentToolCallResult(
            execution.ToolCallId,
            execution.ToolName,
            execution.ResponseSchemaVersion,
            execution.Policy.Decision,
            execution.ExecutionStatus,
            execution.ResultText,
            execution.ErrorCode);
    }

    private void LogExecutionFailure(
        AgentToolExecutionResult execution,
        AgenticChatSession session)
    {
        if (execution.Exception is null)
        {
            return;
        }

        logger.LogError(
            execution.Exception,
            "Agent tool execution failed. ToolName={ToolName} ToolCallId={ToolCallId} ConversationId={ConversationId} CorrelationId={CorrelationId}",
            execution.ToolName,
            execution.ToolCallId,
            session.ConversationId,
            session.Settings.CorrelationId);
    }
}
