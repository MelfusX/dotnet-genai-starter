using GenAIPlatform.Mcp.Tools.Schemas;

namespace GenAIPlatform.Mcp.Tools;

public static class CurrentUserProfileFormatter
{
    public static string ToMarkdown(CurrentUserProfileResponse response) =>
        string.Join(
            Environment.NewLine,
            "# Current User Profile",
            $"- userId: {response.UserId ?? ""}",
            $"- tenantId: {response.TenantId ?? ""}",
            $"- roles: {FormatList(response.Roles)}",
            $"- groups: {FormatList(response.Groups)}");

    private static string FormatList(IReadOnlyCollection<string> values) =>
        values.Count == 0 ? "none" : string.Join(", ", values.Order(StringComparer.OrdinalIgnoreCase));
}
