using System.Globalization;
using GenAIPlatform.Mcp.Tools.Schemas;

namespace GenAIPlatform.Mcp.Tools;

public static class UsageSummaryFormatter
{
    public static string ToMarkdown(UsageSummaryResponse response) =>
        string.Join(
            Environment.NewLine,
            "# Usage Summary",
            $"- requests: {response.Requests.ToString(CultureInfo.InvariantCulture)}",
            $"- inputTokens: {response.InputTokens.ToString(CultureInfo.InvariantCulture)}",
            $"- outputTokens: {response.OutputTokens.ToString(CultureInfo.InvariantCulture)}",
            $"- embeddingTokens: {response.EmbeddingTokens.ToString(CultureInfo.InvariantCulture)}",
            $"- estimatedCost: {response.EstimatedCost.ToString(CultureInfo.InvariantCulture)}",
            $"- currency: {response.Currency}");
}
