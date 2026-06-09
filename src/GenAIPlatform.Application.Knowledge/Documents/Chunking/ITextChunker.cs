using GenAIPlatform.Domain.Documents;

namespace GenAIPlatform.Application.Knowledge.Documents;

public interface ITextChunker
{
    IReadOnlyList<DocumentChunk> Chunk(
        Document document,
        string text,
        DateTimeOffset createdAtUtc);
}
