using Ecommerce.Auth.Domain.Users;
using FluentValidation;

namespace Ecommerce.Auth.Application.Users.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public const int PasswordMinLength = 8;
    public const int PasswordMaxLength = 128;

    public RegisterUserCommandValidator()
    {
        RuleFor(command => command.Email)
            .NotEmpty()
            .MaximumLength(Email.MaxLength)
            .Must(BeValidEmail)
            .WithMessage("Email is invalid.");

        RuleFor(command => command.Password)
            .NotEmpty()
            .MinimumLength(PasswordMinLength)
            .MaximumLength(PasswordMaxLength);
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
