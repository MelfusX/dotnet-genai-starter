using GenAIPlatform.Application.Core.Security;
using Microsoft.Extensions.Options;

namespace GenAIPlatform.Mcp.Security;

public sealed class McpUserContext(IOptionsSnapshot<McpIdentityOptions> options) : IBackgroundUserContext
{
    public bool IsAuthenticated => true;

    public string? UserId => options.Value.UserId;

    public string? TenantId => options.Value.TenantId;

    public IReadOnlyCollection<string> Roles => DistinctValues(options.Value.Roles);

    public IReadOnlyCollection<string> Groups => DistinctValues(options.Value.Groups);

    private static IReadOnlyCollection<string> DistinctValues(IEnumerable<string> values) =>
        values
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
}
