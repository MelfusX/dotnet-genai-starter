using GenAIPlatform.Mcp.Tools.Schemas;

namespace GenAIPlatform.Mcp.Tools;

public static class ServerInfoFormatter
{
    public static string ToMarkdown(ServerInfoResponse response) =>
        string.Join(
            Environment.NewLine,
            "# GenAIPlatform MCP Server",
            $"- hostVersion: {response.HostVersion}",
            $"- userId: {response.UserId ?? ""}",
            $"- tenantId: {response.TenantId ?? ""}",
            $"- roles: {FormatList(response.Roles)}",
            $"- groups: {FormatList(response.Groups)}");

    private static string FormatList(IReadOnlyCollection<string> values) =>
        values.Count == 0 ? "none" : string.Join(", ", values.Order(StringComparer.OrdinalIgnoreCase));
}
