using MediatR;

namespace Ecommerce.Auth.Application.Users.RegisterUser;

public sealed record RegisterUserCommand(
    string Email,
    string Password) : IRequest<RegisterUserResult>;
