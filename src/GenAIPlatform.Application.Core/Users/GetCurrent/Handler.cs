using GenAIPlatform.Application.Core.Dispatching;
using GenAIPlatform.Application.Core.Security;

namespace GenAIPlatform.Application.Core.Users;

public sealed class GetCurrentUserHandler(IUserContext userContext)
    : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
{
    public Task<CurrentUserDto> HandleAsync(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = new CurrentUserDto(
            userContext.IsAuthenticated,
            userContext.UserId,
            userContext.TenantId,
            userContext.Roles,
            userContext.Groups);

        return Task.FromResult(user);
    }
}
