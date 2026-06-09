namespace GenAIPlatform.Domain.Agentic;

public sealed record ToolPolicyDecision(
    ToolRisk Risk,
    string Decision,
    string Reason,
    bool RequiresApproval,
    bool MayExecute);
