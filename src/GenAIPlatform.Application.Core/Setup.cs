using GenAIPlatform.Application.Core.Configuration;
using GenAIPlatform.Application.Core.Dispatching;
using GenAIPlatform.Application.Core.Health;
using GenAIPlatform.Application.Core.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace GenAIPlatform.Application.Core;

public static class Setup
{
    public static IServiceCollection AddApplicationCore(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplicationCoreOptions(configuration);
        services.TryAddSingleton(TimeProvider.System);
        services.TryAddScoped<IApplicationDispatcher, ApplicationDispatcher>();
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DispatchLoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
        services.AddHealthCore();
        services.AddUsersCore();

        return services;
    }

    private static IServiceCollection AddApplicationCoreOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<ApplicationOptions>()
            .Bind(configuration.GetSection(ApplicationOptions.SectionName))
            .ValidateOnStart();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<
            IValidateOptions<ApplicationOptions>,
            ApplicationOptionsValidator>());

        return services;
    }
}
