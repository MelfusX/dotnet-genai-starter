namespace GenAIPlatform.Domain.Evaluations;

public static class EvaluationStatusNames
{
    public static string ToPublicValue(this EvaluationRunStatus status)
    {
        return status.ToString();
    }

    public static string ToPublicValue(this EvaluationCaseStatus status)
    {
        return status.ToString();
    }
}
