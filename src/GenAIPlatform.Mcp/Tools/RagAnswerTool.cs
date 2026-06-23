using System.ComponentModel;
using GenAIPlatform.Application.Core.Dispatching;
using GenAIPlatform.Application.Core.Exceptions;
using GenAIPlatform.Application.Generation.Chat;
using GenAIPlatform.Mcp.Tools.Schemas;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace GenAIPlatform.Mcp.Tools;

[McpServerToolType]
public sealed class RagAnswerTool(IApplicationDispatcher dispatcher)
{
    [McpServerTool(Name = "rag_answer", ReadOnly = true, Idempotent = false, OpenWorld = false)]
    [Description("Answers a question using existing permission-aware RAG retrieval.")]
    public async Task<string> AnswerAsync(
        [Description("Question to answer from indexed document context.")] string question,
        [Description("Optional number of chunks to retrieve.")] int? topK = null,
        [Description("Optional minimum vector similarity score between -1 and 1.")] double? minSimilarityScore = null,
        [Description("Optional document IDs to restrict retrieval after normal authorization filters.")] Guid[]? documentIds = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await dispatcher.DispatchAsync<RagChatCommand, RagChatResponse>(
                new RagChatCommand(
                    question,
                    TopK: topK,
                    MinSimilarityScore: minSimilarityScore,
                    DocumentIds: documentIds),
                cancellationToken);

            return RagAnswerFormatter.ToMarkdown(ToResponse(response));
        }
        catch (RequestValidationException exception)
        {
            throw new McpException($"rag_answer validation failed: {exception.Message}");
        }
        catch (AppException exception)
        {
            throw new McpException($"rag_answer failed: {exception.Message}");
        }
    }

    private static RagAnswerResponse ToResponse(RagChatResponse response) =>
        new(
            response.Message,
            response.NoContext,
            response.CorrelationId,
            response.Citations.Select(ToCitation).ToArray());

    private static RagAnswerCitation ToCitation(RagCitation citation) =>
        new(
            citation.ReferenceId,
            citation.DocumentId,
            citation.ChunkId,
            citation.DocumentVersion,
            citation.ChunkPosition,
            citation.Title,
            citation.FileName,
            citation.SimilarityScore);
}
