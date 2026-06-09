using GenAIPlatform.Domain.Observability;

namespace GenAIPlatform.Infrastructure.Observability;

public interface IPricingRepository
{
    Task<PricingRecord?> GetEffectivePricingAsync(
        string provider,
        string model,
        DateTimeOffset usedAtUtc,
        CancellationToken cancellationToken);
}
