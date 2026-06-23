using System.ComponentModel;
using GenAIPlatform.Application.Core.Dispatching;
using GenAIPlatform.Application.Core.Exceptions;
using GenAIPlatform.Application.Usage.GetUsage;
using GenAIPlatform.Mcp.Tools.Schemas;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace GenAIPlatform.Mcp.Tools;

[McpServerToolType]
public sealed class UsageTool(IApplicationDispatcher dispatcher)
{
    [McpServerTool(Name = "get_usage", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Returns tenant-scoped AI request usage totals.")]
    public async Task<string> GetUsageAsync(
        [Description("Optional inclusive UTC start timestamp.")] DateTimeOffset? fromUtc = null,
        [Description("Optional inclusive UTC end timestamp.")] DateTimeOffset? toUtc = null,
        [Description("Optional user filter; non-admin callers are scoped to the active user.")] string? userId = null,
        [Description("Optional tenant filter; non-admin callers must match the active tenant.")] string? tenantId = null,
        [Description("Optional model filter.")] string? model = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var summary = await dispatcher.DispatchAsync<UsageQuery, UsageSummary>(
                new UsageQuery(fromUtc, toUtc, userId, tenantId, model),
                cancellationToken);

            return UsageSummaryFormatter.ToMarkdown(ToResponse(summary));
        }
        catch (AppException exception)
        {
            throw new McpException($"get_usage failed: {exception.Message}");
        }
    }

    private static UsageSummaryResponse ToResponse(UsageSummary summary) =>
        new(
            summary.Requests,
            summary.InputTokens,
            summary.OutputTokens,
            summary.EmbeddingTokens,
            summary.EstimatedCost,
            summary.Currency);
}
