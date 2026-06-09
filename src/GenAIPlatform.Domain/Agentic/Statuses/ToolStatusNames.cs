namespace GenAIPlatform.Domain.Agentic;

public static class ToolStatusNames
{
    public static string ToPublicValue(this ToolExecutionStatus status)
    {
        return status.ToString();
    }

    public static string ToPublicValue(this ToolApprovalState status)
    {
        return status.ToString();
    }

    public static string ToPublicValue(this ToolValidationStatus status)
    {
        return status.ToString();
    }
}
