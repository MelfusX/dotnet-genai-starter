using GenAIPlatform.Application.Generation.Prompts.Rendering;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GenAIPlatform.Application.Generation.Prompts;

internal static class PromptsSetup
{
    public static IServiceCollection AddPromptsGeneration(this IServiceCollection services)
    {
        services.TryAddSingleton<IPromptTemplateProvider, InMemoryPromptTemplateProvider>();
        services.TryAddScoped<IPromptRenderer, PromptRenderer>();

        return services;
    }
}
