using System.Globalization;
using System.Text;
using GenAIPlatform.Mcp.Tools.Schemas;

namespace GenAIPlatform.Mcp.Tools;

public static class RagAnswerFormatter
{
    public static string ToMarkdown(RagAnswerResponse response)
    {
        var builder = new StringBuilder();
        builder.AppendLine(response.Message);
        builder.AppendLine();
        builder.AppendLine($"noContext: {response.NoContext.ToString().ToLowerInvariant()}");
        builder.AppendLine($"correlationId: {response.CorrelationId}");

        if (response.Citations.Count == 0)
        {
            builder.AppendLine("citations: none");
            return builder.ToString().TrimEnd();
        }

        builder.AppendLine("citations:");
        foreach (var citation in response.Citations)
        {
            builder.AppendLine(
                CultureInfo.InvariantCulture,
                $"- [{citation.ReferenceId}] {citation.Title} ({citation.FileName}), documentId={citation.DocumentId}, chunkId={citation.ChunkId}, version={citation.DocumentVersion}, chunk={citation.ChunkPosition}, score={citation.SimilarityScore:0.###}");
        }

        return builder.ToString().TrimEnd();
    }
}
