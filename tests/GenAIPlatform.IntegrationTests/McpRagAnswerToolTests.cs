extern alias McpHost;
using FluentValidation.Results;
using GenAIPlatform.Application.Core.Dispatching;
using GenAIPlatform.Application.Generation.Chat;
using McpHost::GenAIPlatform.Mcp.Tools;
using ModelContextProtocol;

namespace GenAIPlatform.IntegrationTests;

public sealed class McpRagAnswerToolTests
{
    [Fact]
    public async Task AnswerAsync_FormatsMessageAndCitations()
    {
        var documentId = Guid.NewGuid();
        var chunkId = Guid.NewGuid();
        var tool = new RagAnswerTool(new StubDispatcher(new RagChatResponse(
            "The document says inspect access filters. [1]",
            "mock-chat",
            "mock",
            Usage: null,
            Prompt: null,
            "rag-test",
            NoContext: false,
            [new RagCitation("1", documentId, chunkId, 1, 0, "Security Notes", "security.md", 0.91)])));

        var markdown = await tool.AnswerAsync("What does the document say?", topK: 3);

        Assert.Contains("The document says inspect access filters. [1]", markdown, StringComparison.Ordinal);
        Assert.Contains("noContext: false", markdown, StringComparison.Ordinal);
        Assert.Contains($"documentId={documentId}", markdown, StringComparison.Ordinal);
        Assert.Contains("Security Notes (security.md)", markdown, StringComparison.Ordinal);
    }

    [Fact]
    public async Task AnswerAsync_FormatsNoContextExplicitly()
    {
        var tool = new RagAnswerTool(new StubDispatcher(new RagChatResponse(
            "I could not find relevant document context for that question.",
            "mock-chat",
            Provider: null,
            Usage: null,
            Prompt: null,
            "rag-no-context",
            NoContext: true,
            [])));

        var markdown = await tool.AnswerAsync("What is not in the corpus?");

        Assert.Contains("noContext: true", markdown, StringComparison.Ordinal);
        Assert.Contains("citations: none", markdown, StringComparison.Ordinal);
        Assert.Contains("I could not find relevant document context", markdown, StringComparison.Ordinal);
    }

    [Fact]
    public async Task AnswerAsync_MapsValidationFailureToMcpError()
    {
        var tool = new RagAnswerTool(new StubDispatcher(
            new RequestValidationException([new ValidationFailure("Message", "Message must not be empty.")])));

        var exception = await Assert.ThrowsAsync<McpException>(() => tool.AnswerAsync(""));

        Assert.Contains("rag_answer validation failed", exception.Message, StringComparison.Ordinal);
    }

    private sealed class StubDispatcher(object result) : IApplicationDispatcher
    {
        public Task<TResponse> DispatchAsync<TRequest, TResponse>(
            TRequest request,
            CancellationToken cancellationToken = default)
            where TRequest : IRequest<TResponse>
        {
            if (result is Exception exception)
            {
                return Task.FromException<TResponse>(exception);
            }

            Assert.IsType<RagChatCommand>(request);
            return Task.FromResult((TResponse)result);
        }
    }
}
