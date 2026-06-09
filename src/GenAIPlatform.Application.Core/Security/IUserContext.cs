namespace GenAIPlatform.Application.Core.Security;

public interface IUserContext
{
    bool IsAuthenticated { get; }

    string? UserId { get; }

    string? TenantId { get; }

    IReadOnlyCollection<string> Roles { get; }

    IReadOnlyCollection<string> Groups { get; }
}
