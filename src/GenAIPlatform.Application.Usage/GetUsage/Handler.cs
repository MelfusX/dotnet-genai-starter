using GenAIPlatform.Application.Core.Dispatching;

namespace GenAIPlatform.Application.Usage.GetUsage;

public sealed class UsageQueryHandler(
    IUsageRepository usageRepository,
    UsageQueryScopeResolver scopeResolver)
    : IRequestHandler<UsageQuery, UsageSummary>
{
    public Task<UsageSummary> HandleAsync(
        UsageQuery request,
        CancellationToken cancellationToken)
    {
        return usageRepository.GetUsageAsync(
            scopeResolver.Resolve(request),
            cancellationToken);
    }
}
