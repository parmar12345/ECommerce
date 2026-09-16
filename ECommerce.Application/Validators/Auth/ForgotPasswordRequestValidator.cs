using ECommerce.Application.DTOs.Auth;
using FluentValidation;

namespace ECommerce.Application.Validators.Auth;

public class ForgotPasswordRequestValidator
    : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);
    }
}