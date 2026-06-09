namespace GenAIPlatform.Application.Core.Users;

public sealed record CurrentUserDto(
    bool IsAuthenticated,
    string? UserId,
    string? TenantId,
    IReadOnlyCollection<string> Roles,
    IReadOnlyCollection<string> Groups);
