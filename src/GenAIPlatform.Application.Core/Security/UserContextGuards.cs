using GenAIPlatform.Application.Core.Exceptions;

namespace GenAIPlatform.Application.Core.Security;

public static class UserContextGuards
{
    public static string RequireAuthenticatedTenant(this IUserContext context)
    {
        return context.IsAuthenticated && !string.IsNullOrWhiteSpace(context.TenantId)
            ? context.TenantId
            : throw new UnauthorizedRequestException("An authenticated tenant is required.");
    }

    public static string RequireAuthenticatedUser(this IUserContext context)
    {
        return context.IsAuthenticated && !string.IsNullOrWhiteSpace(context.UserId)
            ? context.UserId
            : throw new UnauthorizedRequestException("An authenticated user is required.");
    }
}
