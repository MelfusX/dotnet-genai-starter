using GenAIPlatform.Api.Configuration;
using GenAIPlatform.Application.Core.Security;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GenAIPlatform.Api.Security;

internal static class ApiUserContextSetup
{
    public static IServiceCollection AddApiUserContext(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services
            .AddOptions<DemoAuthOptions>()
            .Bind(configuration.GetSection(DemoAuthOptions.SectionName));

        var demoAuthOptions = configuration
            .GetSection(DemoAuthOptions.SectionName)
            .Get<DemoAuthOptions>() ?? new DemoAuthOptions();

        if (!ShouldUseDemoAuth(environment, demoAuthOptions))
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IStartupFilter, ApiUserContextStartupFilter>());
            return services;
        }

        // Demo API auth explicitly replaces any test or host-provided foreground
        // adapter so the local sample path is deterministic.
        services.RemoveAll<IUserContext>();
        services.AddScoped<IUserContext, DemoHeaderUserContext>();

        return services;
    }

    private static bool ShouldUseDemoAuth(IHostEnvironment environment, DemoAuthOptions options)
    {
        if (!options.Enabled)
        {
            return false;
        }

        if (environment.IsProduction())
        {
            return false;
        }

        return environment.IsDevelopment() || options.AllowInNonDevelopment;
    }

    internal static InvalidOperationException CreateMissingUserContextException() =>
        new("API user context is not configured. Register a real IUserContext authentication adapter for deployed API hosts. Demo header auth is limited to Development and explicit non-production demo environments; see docs/security-model.md.");
}
