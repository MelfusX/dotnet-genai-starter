using GenAIPlatform.Application.Core.Security;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GenAIPlatform.Evaluations;

public static class Setup
{
    public static IServiceCollection AddEvaluations(this IServiceCollection services)
    {
        services.RemoveAll<IUserContext>();
        services.AddScoped<IUserContext, EvaluationCliUserContext>();

        return services;
    }
}
