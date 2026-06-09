using GenAIPlatform.Application.Agentic.Chat;
using GenAIPlatform.Application.Evaluations.StartRun.Cases;
using GenAIPlatform.Application.Generation.ModelGateway;
using GenAIPlatform.Application.Knowledge.Embeddings;
using GenAIPlatform.Infrastructure.Observability.Logging;
using GenAIPlatform.Infrastructure.Observability.Pricing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace GenAIPlatform.Infrastructure.Observability;

public static class ObservabilitySetup
{
    public static IServiceCollection AddObservabilityInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddObservabilityOptions(configuration);
        services.TryAddScoped<AiCostEstimator>();
        services.TryAddScoped<AiRequestLogWriter>();
        services.TryAddScoped<AiModelRequestLoggingService>();
        services.Replace(ServiceDescriptor.Scoped<IAiModelRequestLogger, AiModelRequestLogger>());
        services.Replace(ServiceDescriptor.Scoped<IAgenticCostEstimator, AgenticCostEstimator>());
        services.Replace(ServiceDescriptor.Scoped<IEvaluationCostEstimator, EvaluationCostEstimator>());
        services.Replace(ServiceDescriptor.Scoped<IDiscardedEmbeddingUsageLogger, DiscardedEmbeddingUsageLogger>());

        return services;
    }

    private static IServiceCollection AddObservabilityOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<AiRequestLoggingOptions>()
            .Bind(configuration.GetSection(AiRequestLoggingOptions.SectionName))
            .ValidateOnStart();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<
            IValidateOptions<AiRequestLoggingOptions>,
            AiRequestLoggingOptionsValidator>());
        return services;
    }
}
