using FluentValidation;
using GenAIPlatform.Application.Knowledge.Documents;
using GenAIPlatform.Application.Knowledge.Embeddings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace GenAIPlatform.Application.Knowledge;

public static class Setup
{
    public static IServiceCollection AddKnowledgeApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddKnowledgeOptions(configuration);
        services.TryAddScoped<IDiscardedEmbeddingUsageLogger, NoopDiscardedEmbeddingUsageLogger>();
        services.AddValidatorsFromAssembly(typeof(Setup).Assembly, includeInternalTypes: true);
        services.AddDocumentsKnowledge();

        return services;
    }

    private static IServiceCollection AddKnowledgeOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<DocumentIngestionOptions>()
            .Bind(configuration.GetSection(DocumentIngestionOptions.SectionName))
            .ValidateOnStart();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<
            IValidateOptions<DocumentIngestionOptions>,
            DocumentIngestionOptionsValidator>());
        services
            .AddOptions<EmbeddingOptions>()
            .Bind(configuration.GetSection(EmbeddingOptions.SectionName))
            .ValidateOnStart();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<
            IValidateOptions<EmbeddingOptions>,
            EmbeddingOptionsValidator>());

        return services;
    }
}
