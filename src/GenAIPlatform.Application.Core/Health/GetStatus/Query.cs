using GenAIPlatform.Application.Core.Dispatching;

namespace GenAIPlatform.Application.Core.Health;

public sealed record GetHealthStatusQuery(string Component) : IRequest<HealthStatus>;
