using FluentValidation;
using GenAIPlatform.Application.Generation.Chat;
using GenAIPlatform.Application.Generation.ModelGateway;
using GenAIPlatform.Application.Generation.Prompts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace GenAIPlatform.Application.Generation;

public static class Setup
{
    public static IServiceCollection AddGenerationApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddGenerationOptions(configuration);
        services.TryAddScoped<ModelGatewayRequestPolicy>();
        services.TryAddScoped<IAiModelRequestLogger, NoopAiModelRequestLogger>();
        services.AddValidatorsFromAssembly(typeof(Setup).Assembly, includeInternalTypes: true);
        services.AddPromptsGeneration();
        services.AddChatGeneration();

        return services;
    }

    private static IServiceCollection AddGenerationOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<RagOptions>()
            .Bind(configuration.GetSection(RagOptions.SectionName))
            .ValidateOnStart();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<
            IValidateOptions<RagOptions>,
            RagOptionsValidator>());
        services
            .AddOptions<ModelGatewayOptions>()
            .Bind(configuration.GetSection(ModelGatewayOptions.SectionName))
            .ValidateOnStart();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<
            IValidateOptions<ModelGatewayOptions>,
            ModelGatewayOptionsValidator>());

        return services;
    }
}
