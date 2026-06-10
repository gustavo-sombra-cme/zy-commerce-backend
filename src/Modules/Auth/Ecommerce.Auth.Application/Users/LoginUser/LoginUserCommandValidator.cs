using Ecommerce.Auth.Domain.Users;
using FluentValidation;

namespace Ecommerce.Auth.Application.Users.LoginUser;

public sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .MaximumLength(Email.MaxLength)
            .Must(BeValidEmail)
            .WithMessage("Email is invalid.");

        RuleFor(command => command.Password)
            .NotEmpty();
    }

    private static bool BeValidEmail(string email)
    {
        try
        {
            Email.Create(email);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
