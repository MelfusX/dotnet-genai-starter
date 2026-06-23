namespace GenAIPlatform.Mcp.Security;

public sealed class McpIdentityOptions
{
    public const string SectionName = "GenAIPlatform:Mcp:Identity";

    public string UserId { get; set; } = "mcp-user";

    public string TenantId { get; set; } = "local";

    public string[] Roles { get; set; } = ["developer"];

    public string[] Groups { get; set; } = [];
}
