extern alias McpHost;
using GenAIPlatform.Application.Core.Configuration;
using GenAIPlatform.Application.Core.Security;
using McpHost::GenAIPlatform.Mcp.Tools;
using Microsoft.Extensions.Options;

namespace GenAIPlatform.IntegrationTests;

public sealed class McpServerInfoToolTests
{
    [Fact]
    public void GetServerInfo_UsesCurrentScopedIdentity()
    {
        var tool = new ServerInfoTool(
            new TestUserContext("tenant-a", "user-a", ["reader", "developer"]),
            Options.Create(new ApplicationOptions
            {
                ApiVersion = "v1",
                RunnerVersion = "0.2.0-test"
            }));

        var markdown = tool.GetServerInfo();

        Assert.Contains("hostVersion: 0.2.0-test", markdown, StringComparison.Ordinal);
        Assert.Contains("userId: user-a", markdown, StringComparison.Ordinal);
        Assert.Contains("tenantId: tenant-a", markdown, StringComparison.Ordinal);
        Assert.Contains("roles: developer, reader", markdown, StringComparison.Ordinal);
    }

    private sealed class TestUserContext(
        string tenantId,
        string userId,
        IReadOnlyCollection<string> roles) : IUserContext
    {
        public bool IsAuthenticated => true;

        public string? UserId => userId;

        public string? TenantId => tenantId;

        public IReadOnlyCollection<string> Roles => roles;

        public IReadOnlyCollection<string> Groups => [];
    }
}
