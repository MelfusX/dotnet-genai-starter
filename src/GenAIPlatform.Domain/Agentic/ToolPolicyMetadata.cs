namespace GenAIPlatform.Domain.Agentic;

public sealed record ToolPolicyMetadata(
    ToolRisk Risk,
    string Decision,
    string Reason,
    bool RequiresApproval,
    bool MayExecute)
{
    public static ToolPolicyMetadata Allowed(string reason)
    {
        return new ToolPolicyMetadata(
            ToolRisk.Safe,
            "Allowed",
            reason,
            RequiresApproval: false,
            MayExecute: true);
    }

    public static ToolPolicyMetadata ApprovalRequired(string reason)
    {
        return new ToolPolicyMetadata(
            ToolRisk.Risky,
            "RequiresApproval",
            reason,
            RequiresApproval: true,
            MayExecute: true);
    }

    public ToolPolicyDecision ToDecision()
    {
        return new ToolPolicyDecision(
            Risk,
            Decision,
            Reason,
            RequiresApproval,
            MayExecute);
    }
}
