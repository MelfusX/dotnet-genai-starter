using GenAIPlatform.Application.Core.Security;
using Microsoft.Extensions.DependencyInjection;

namespace GenAIPlatform.Worker;

public static class Setup
{
    public static IServiceCollection AddWorker(this IServiceCollection services)
    {
        services.AddScoped<IUserContext>(
            serviceProvider => serviceProvider.GetRequiredService<IBackgroundUserContext>());
        services.AddHostedService<Worker>();

        return services;
    }
}
