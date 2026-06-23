extern alias McpHost;
using GenAIPlatform.Application.Core.Dispatching;
using GenAIPlatform.Application.Usage.GetUsage;
using McpHost::GenAIPlatform.Mcp.Tools;
using ModelContextProtocol;

namespace GenAIPlatform.IntegrationTests;

public sealed class McpUsageToolTests
{
    [Fact]
    public async Task GetUsageAsync_FormatsUsageSummary()
    {
        var tool = new UsageTool(new StubDispatcher(new UsageSummary(
            Requests: 3,
            InputTokens: 120,
            OutputTokens: 45,
            EmbeddingTokens: 30,
            EstimatedCost: 0.0123m,
            Currency: "USD")));

        var markdown = await tool.GetUsageAsync(model: "mock-chat");

        Assert.Contains("# Usage Summary", markdown, StringComparison.Ordinal);
        Assert.Contains("requests: 3", markdown, StringComparison.Ordinal);
        Assert.Contains("inputTokens: 120", markdown, StringComparison.Ordinal);
        Assert.Contains("estimatedCost: 0.0123", markdown, StringComparison.Ordinal);
        Assert.Contains("currency: USD", markdown, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GetUsageAsync_MapsApplicationValidationToMcpError()
    {
        var tool = new UsageTool(new StubDispatcher(
            new UsageQueryValidationException("from must be before or equal to to.")));

        var exception = await Assert.ThrowsAsync<McpException>(() => tool.GetUsageAsync(
            DateTimeOffset.Parse("2026-06-22T00:00:00Z"),
            DateTimeOffset.Parse("2026-06-21T00:00:00Z")));

        Assert.Contains("get_usage failed", exception.Message, StringComparison.Ordinal);
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

            Assert.IsType<UsageQuery>(request);
            return Task.FromResult((TResponse)result);
        }
    }
}
