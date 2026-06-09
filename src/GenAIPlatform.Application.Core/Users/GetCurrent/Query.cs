using GenAIPlatform.Application.Core.Dispatching;

namespace GenAIPlatform.Application.Core.Users;

public sealed record GetCurrentUserQuery : IRequest<CurrentUserDto>;
