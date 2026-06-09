using GenAIPlatform.Application.Core.Dispatching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GenAIPlatform.Application.Generation.Chat;

internal static class ChatSetup
{
    public static IServiceCollection AddChatGeneration(this IServiceCollection services)
    {
        services.TryAddScoped<RagChatNormalizer>();
        services.TryAddScoped<RagPromptBuilder>();
        services.TryAddScoped<RagContextBudgetResolver>();
        services.TryAddScoped<RagRetrievalPipeline>();
        services.TryAddScoped<RagNoContextResponseFactory>();
        services.TryAddScoped<IRequestHandler<DirectChatCommand, DirectChatResponse>, DirectChatHandler>();
        services.TryAddScoped<IRequestHandler<RagChatCommand, RagChatResponse>, RagChatHandler>();

        return services;
    }
}
