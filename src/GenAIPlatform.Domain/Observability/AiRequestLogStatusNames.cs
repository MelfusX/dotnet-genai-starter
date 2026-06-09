namespace GenAIPlatform.Domain.Observability;

public static class AiRequestLogStatusNames
{
    public static string ToPublicValue(this AiRequestLogStatus status)
    {
        return status.ToString();
    }
}
