namespace GenAIPlatform.Application.Usage.GetUsage;

public interface IUsageRepository
{
    Task<UsageSummary> GetUsageAsync(
        UsageQuery query,
        CancellationToken cancellationToken);
}
