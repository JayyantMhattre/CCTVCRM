using FluentValidation;

namespace Ashraak.Auth.Application.Commands.RegisterUser;

/// <summary>
/// FluentValidation validator for <see cref="RegisterUserCommand"/>.
/// Runs as part of the <c>ValidationBehavior</c> MediatR pipeline.
/// Validation failures are returned as a <c>422 Unprocessable Entity</c> response.
/// </summary>
/// <remarks>
/// Password rules enforced here match the configuration in <c>AuthModule.AddAuthModule</c>
/// (ASP.NET Core Identity options) to provide early, user-friendly feedback
/// before reaching the password hasher.
/// </remarks>
internal sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    /// <summary>
    /// Defines all validation rules for <see cref="RegisterUserCommand"/>.
    /// </summary>
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.TenantId)
            .NotEmpty()
            .WithMessage("Tenant identifier is required.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(254)
            .WithMessage("A valid email address of up to 254 characters is required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(128)
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");

        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Display name is required and must be 100 characters or fewer.");
    }
}
