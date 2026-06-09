namespace GenAIPlatform.Domain.Agentic;

public sealed class ToolPolicy
{
    // Keeps audit classification explicit for well-known destructive names even
    // when no matching tool is registered in the demo registry.
    private static readonly HashSet<string> KnownForbiddenUnregisteredToolNames = new(StringComparer.Ordinal)
    {
        "SendEmail",
        "DeleteDocument",
        "RunSqlQuery"
    };

    public ToolPolicyDecision Decide(ToolPolicyMetadata? policy, string requestedToolName)
    {
        if (policy is not null)
        {
            return policy.ToDecision();
        }

        return KnownForbiddenUnregisteredToolNames.Contains(requestedToolName)
            ? new ToolPolicyDecision(
                ToolRisk.Forbidden,
                "Forbidden",
                "The backend policy forbids this tool for model-proposed execution.",
                RequiresApproval: false,
                MayExecute: false)
            : new ToolPolicyDecision(
                ToolRisk.Forbidden,
                "UnknownTool",
                "The requested tool is not registered for this user/request.",
                RequiresApproval: false,
                MayExecute: false);
    }
}
