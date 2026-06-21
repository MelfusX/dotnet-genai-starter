using GenAIPlatform.Application.Core.Security;
using Microsoft.Extensions.Options;

namespace GenAIPlatform.Mcp.Security;

public sealed class McpUserContext(IOptionsSnapshot<McpIdentityOptions> options) : IBackgroundUserContext
{
    public bool IsAuthenticated => true;

    public string? UserId => options.Value.UserId;

    public string? TenantId => options.Value.TenantId;

    public IReadOnlyCollection<string> Roles => options.Value.Roles;

    public IReadOnlyCollection<string> Groups => options.Value.Groups;
}
