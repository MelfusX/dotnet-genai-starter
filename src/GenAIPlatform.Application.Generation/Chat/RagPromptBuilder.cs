using System.Globalization;
using System.Text;
using GenAIPlatform.Application.Knowledge.Retrieval;

namespace GenAIPlatform.Application.Generation.Chat;

public sealed class RagPromptBuilder
{
    private const int MaxPromptMetadataCharacters = 120;

    public RagPromptContext Build(
        IReadOnlyList<RetrievedDocumentChunk> chunks,
        int maxContextCharacters)
    {
        ArgumentNullException.ThrowIfNull(chunks);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxContextCharacters);

        var context = new StringBuilder();
        var citations = new List<RagCitation>(chunks.Count);

        foreach (var chunk in chunks)
        {
            var referenceId = (citations.Count + 1).ToString(CultureInfo.InvariantCulture);
            var text = chunk.Text.Trim();
            var separator = context.Length > 0 ? Environment.NewLine + Environment.NewLine : string.Empty;
            var entryPrefix = BuildEntryPrefix(referenceId, chunk);
            var remainingTextCharacters =
                maxContextCharacters - context.Length - separator.Length - entryPrefix.Length;

            if (remainingTextCharacters <= 0)
            {
                continue;
            }

            if (text.Length > remainingTextCharacters)
            {
                text = text[..remainingTextCharacters].TrimEnd();
            }

            if (text.Length == 0)
            {
                continue;
            }

            if (context.Length > 0)
            {
                context.Append(separator);
            }

            context.Append(entryPrefix).Append(text);

            citations.Add(new RagCitation(
                referenceId,
                chunk.DocumentId,
                chunk.ChunkId,
                chunk.DocumentVersion,
                chunk.ChunkPosition,
                chunk.Title,
                chunk.FileName,
                chunk.SimilarityScore));
        }

        return new RagPromptContext(context.ToString(), citations);
    }

    private static string BuildEntryPrefix(
        string referenceId,
        RetrievedDocumentChunk chunk)
    {
        var builder = new StringBuilder();
        builder
            .Append('[')
            .Append(referenceId)
            .AppendLine("]")
            .Append("Title: ")
            .AppendLine(NormalizePromptMetadata(chunk.Title))
            .Append("File: ")
            .AppendLine(NormalizePromptMetadata(chunk.FileName))
            .AppendLine("Text:");

        return builder.ToString();
    }

    private static string NormalizePromptMetadata(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = string.Join(
            ' ',
            value.Split(
                (char[]?)null,
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));

        return normalized.Length <= MaxPromptMetadataCharacters
            ? normalized
            : normalized[..MaxPromptMetadataCharacters].TrimEnd();
    }
}
