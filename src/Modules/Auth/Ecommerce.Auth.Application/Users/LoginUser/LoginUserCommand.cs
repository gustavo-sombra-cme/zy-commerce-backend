using MediatR;

namespace Ecommerce.Auth.Application.Users.LoginUser;

public sealed record LoginUserCommand(
    string Email,
    string Password) : IRequest<LoginUserResult>;
