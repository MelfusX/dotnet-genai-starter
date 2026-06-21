extern alias McpHost;
using GenAIPlatform.Application.Core.Dispatching;
using GenAIPlatform.Application.Core.Security;
using McpHost::GenAIPlatform.Mcp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GenAIPlatform.IntegrationTests;

public sealed class McpHostCompositionTests
{
    [Fact]
    public void McpHostServices_CanBuildAndResolveScopedIdentity()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.mcp.test.json", optional: false)
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddGenAIPlatformMcp(configuration);

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        });

        using var scope = provider.CreateScope();
        var userContext = scope.ServiceProvider.GetRequiredService<IUserContext>();
        var backgroundContext = scope.ServiceProvider.GetRequiredService<IBackgroundUserContext>();

        Assert.NotNull(scope.ServiceProvider.GetRequiredService<IApplicationDispatcher>());
        Assert.True(userContext.IsAuthenticated);
        Assert.Same(userContext, backgroundContext);
        Assert.Equal("mcp-user", userContext.UserId);
        Assert.Equal("local", userContext.TenantId);
        Assert.Contains("developer", userContext.Roles);
    }
}


