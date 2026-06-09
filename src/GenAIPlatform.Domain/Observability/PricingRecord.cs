namespace GenAIPlatform.Domain.Observability;

public sealed record PricingRecord(
    Guid Id,
    string Provider,
    string Model,
    string Currency,
    decimal InputTokenPricePerMillion,
    decimal OutputTokenPricePerMillion,
    decimal? EmbeddingTokenPricePerMillion,
    DateTimeOffset EffectiveFromUtc,
    DateTimeOffset? EffectiveToUtc);
