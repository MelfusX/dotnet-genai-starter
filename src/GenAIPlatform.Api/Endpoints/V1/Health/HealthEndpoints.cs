using GenAIPlatform.Application.Core.Dispatching;
using GenAIPlatform.Application.Core.Health;

namespace GenAIPlatform.Api;

internal static class HealthEndpoints
{
    public static RouteGroupBuilder MapHealthEndpoints(this RouteGroupBuilder api)
    {
        api.MapGet("/health", async (
                IApplicationDispatcher dispatcher,
                CancellationToken cancellationToken) =>
            {
                var result = await dispatcher.DispatchAsync<GetHealthStatusQuery, HealthStatus>(
                    new GetHealthStatusQuery("api"),
                    cancellationToken);

                return Results.Ok(result);
            })
            .WithName("GetApiV1Health")
            .WithSummary("Liveness probe for the API host.")
            .Produces<HealthStatus>(StatusCodes.Status200OK);

        return api;
    }
}
