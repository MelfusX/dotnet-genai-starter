namespace GenAIPlatform.Mcp.Tools.Schemas;

public sealed record CurrentUserProfileResponse(
    string? UserId,
    string? TenantId,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Groups)
{
    public static CurrentUserProfileResponse Empty { get; } = new(null, null, [], []);
}
