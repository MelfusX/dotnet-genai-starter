namespace GenAIPlatform.Mcp.Tools.Schemas;

public sealed record ServerInfoResponse(
    string HostVersion,
    string? UserId,
    string? TenantId,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Groups);
