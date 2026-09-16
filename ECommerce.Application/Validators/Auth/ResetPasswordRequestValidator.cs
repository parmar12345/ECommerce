using ECommerce.Application.DTOs.Auth;
using FluentValidation;

namespace ECommerce.Application.Validators.Auth;

public class ResetPasswordRequestValidator
    : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Reset token is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password is required.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.")
            .MaximumLength(100)
            .WithMessage("Password cannot exceed 100 characters.");
    }
}