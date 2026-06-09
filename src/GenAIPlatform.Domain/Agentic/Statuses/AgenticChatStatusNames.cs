namespace GenAIPlatform.Domain.Agentic;

public static class AgenticChatStatusNames
{
    public static string ToPublicValue(this AgenticChatStatus status)
    {
        return status.ToString();
    }
}
