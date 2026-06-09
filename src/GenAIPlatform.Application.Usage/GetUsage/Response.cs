namespace GenAIPlatform.Application.Usage.GetUsage;

public sealed record UsageSummary(
    long Requests,
    long InputTokens,
    long OutputTokens,
    long EmbeddingTokens,
    decimal EstimatedCost,
    string Currency);
