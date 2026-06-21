using Microsoft.Extensions.Options;

namespace GenAIPlatform.Mcp.Security;

public sealed class McpIdentityOptionsValidator : IValidateOptions<McpIdentityOptions>
{
    public ValidateOptionsResult Validate(string? name, McpIdentityOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.UserId))
        {
            return ValidateOptionsResult.Fail(
                "MCP identity configuration is invalid. Configure GenAIPlatform:Mcp:Identity:UserId.");
        }

        if (string.IsNullOrWhiteSpace(options.TenantId))
        {
            return ValidateOptionsResult.Fail(
                "MCP identity configuration is invalid. Configure GenAIPlatform:Mcp:Identity:TenantId.");
        }

        return ValidateOptionsResult.Success;
    }
}
