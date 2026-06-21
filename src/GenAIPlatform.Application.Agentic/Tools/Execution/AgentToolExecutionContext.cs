namespace GenAIPlatform.Application.Agentic.Tools.Execution;

internal sealed record AgentToolExecutionContext(
    Guid ConversationId,
    string TenantId,
    string UserId,
    string CorrelationId,
    string PolicyVersion,
    bool ApproveRiskyTools);
