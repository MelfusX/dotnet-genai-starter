using GenAIPlatform.Application.Core.Dispatching;
using GenAIPlatform.Application.Core.Users;

namespace GenAIPlatform.Api;

internal static class UserEndpoints
{
    public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder api)
    {
        api.MapGet("/users/me", async (
                IApplicationDispatcher dispatcher,
                CancellationToken cancellationToken) =>
            {
                var result = await dispatcher.DispatchAsync<GetCurrentUserQuery, CurrentUserDto>(
                    new GetCurrentUserQuery(),
                    cancellationToken);

                return Results.Ok(result);
            })
            .WithName("GetCurrentUser")
            .WithSummary("Return the authenticated caller's identity (user id, tenant, roles, groups).")
            .Produces<CurrentUserDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        return api;
    }
}
