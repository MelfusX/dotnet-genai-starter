using FluentValidation;
using GenAIPlatform.Application.Agentic.Chat;
using GenAIPlatform.Application.Agentic.Tools;
using GenAIPlatform.Application.Core.Dispatching;
using GenAIPlatform.Domain.Agentic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace GenAIPlatform.Application.Agentic;

public static class Setup
{
    public static IServiceCollection AddAgenticApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddAgenticOptions(configuration);
        services.AddValidatorsFromAssembly(typeof(Setup).Assembly, includeInternalTypes: true);
        services.TryAddScoped<IAgentToolRegistry, DemoAgentToolRegistry>();
        services.TryAddScoped<ToolPolicy>();
        services.TryAddScoped<AgenticPromptBuilder>();
        services.TryAddScoped<AgentToolAuditWriter>();
        services.TryAddScoped<AgentToolExecutor>();
        services.TryAddScoped<AgenticToolCallProcessor>();
        services.TryAddScoped<IAgenticCostEstimator, NoopAgenticCostEstimator>();
        services.TryAddScoped<AgenticBudgetGuard>();
        services.TryAddScoped<AgenticChatLoopRunner>();
        services.TryAddScoped<IRequestHandler<AgenticChatCommand, AgenticChatResponse>, AgenticChatHandler>();

        return services;
    }

    private static IServiceCollection AddAgenticOptions(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<AgenticChatOptions>()
            .Bind(configuration.GetSection(AgenticChatOptions.SectionName))
            .ValidateOnStart();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<
            IValidateOptions<AgenticChatOptions>,
            AgenticChatOptionsValidator>());

        return services;
    }
}
