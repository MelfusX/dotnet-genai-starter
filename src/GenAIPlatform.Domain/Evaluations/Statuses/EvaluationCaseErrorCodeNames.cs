namespace GenAIPlatform.Domain.Evaluations;

public static class EvaluationCaseErrorCodeNames
{
    public static string ToPublicValue(this EvaluationCaseErrorCode errorCode)
    {
        return errorCode.ToString().ToLowerInvariant();
    }
}
