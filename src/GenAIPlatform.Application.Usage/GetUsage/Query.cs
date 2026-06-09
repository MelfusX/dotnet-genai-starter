using GenAIPlatform.Application.Core.Dispatching;

namespace GenAIPlatform.Application.Usage.GetUsage;

public sealed record UsageQuery(
    DateTimeOffset? FromUtc = null,
    DateTimeOffset? ToUtc = null,
    string? UserId = null,
    string? TenantId = null,
    string? Model = null)
    : IRequest<UsageSummary>;
