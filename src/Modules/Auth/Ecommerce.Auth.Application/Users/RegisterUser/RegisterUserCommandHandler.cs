using Ecommerce.Auth.Application.Abstractions;
using Ecommerce.Auth.Application.Security;
using Ecommerce.Auth.Application.Users;
using Ecommerce.Auth.Domain.Users;
using MediatR;

namespace Ecommerce.Auth.Application.Users.RegisterUser;

public sealed class RegisterUserCommandHandler(
    IUserRepository userRepository,
    IAuthUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher)
    : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Create(request.Email);

        if (await userRepository.ExistsByEmailAsync(email, cancellationToken))
        {
            throw new DuplicateEmailException(email.Value);
        }

        var hashedPassword = passwordHasher.Hash(request.Password);
        var passwordHash = PasswordHash.Create(hashedPassword);
        var user = User.Register(UserId.New(), email, passwordHash, DateTimeOffset.UtcNow);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterUserResult(user.Id.Value, user.Email.Value);
    }
}
