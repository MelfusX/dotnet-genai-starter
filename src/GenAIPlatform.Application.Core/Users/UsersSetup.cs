using GenAIPlatform.Application.Core.Dispatching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GenAIPlatform.Application.Core.Users;

internal static class UsersSetup
{
    public static IServiceCollection AddUsersCore(this IServiceCollection services)
    {
        services.TryAddScoped<IRequestHandler<GetCurrentUserQuery, CurrentUserDto>, GetCurrentUserHandler>();

        return services;
    }
}
