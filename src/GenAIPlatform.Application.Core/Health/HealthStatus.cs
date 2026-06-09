namespace GenAIPlatform.Application.Core.Health;

public sealed record HealthStatus(
    string Status,
    string Component,
    string ApiVersion,
    DateTimeOffset CheckedAtUtc);
