using Ecommerce.Auth.Application.Security;
using Ecommerce.Auth.Application.Users;
using Ecommerce.Auth.Domain.Users;
using MediatR;

namespace Ecommerce.Auth.Application.Users.LoginUser;

public sealed class LoginUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IAccessTokenGenerator accessTokenGenerator)
    : IRequestHandler<LoginUserCommand, LoginUserResult>
{
    public async Task<LoginUserResult> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Create(request.Email);
        var user = await userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            throw new InvalidCredentialsException();
        }

        if (!user.IsActive)
        {
            throw new InactiveUserException(email.Value);
        }

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw new InvalidCredentialsException();
        }

        var accessToken = accessTokenGenerator.Generate(user);

        return new LoginUserResult(
            user.Id.Value,
            user.Email.Value,
            accessToken.AccessToken,
            accessToken.TokenType,
            accessToken.ExpiresAt);
    }
}
