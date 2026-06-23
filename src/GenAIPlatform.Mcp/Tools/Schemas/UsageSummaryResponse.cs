namespace GenAIPlatform.Mcp.Tools.Schemas;

public sealed record UsageSummaryResponse(
    long Requests,
    long InputTokens,
    long OutputTokens,
    long EmbeddingTokens,
    decimal EstimatedCost,
    string Currency);
