using ECommerce.Application.DTOs.Addresses;
using FluentValidation;

namespace ECommerce.Application.Validators.Addresses;

public class CreateAddressRequestValidator
    : AbstractValidator<CreateAddressRequest>
{
    public CreateAddressRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.AddressLine1)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.AddressLine2)
            .MaximumLength(200);

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.State)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Country)
            .NotEmpty()
            .MaximumLength(100);
    }
}