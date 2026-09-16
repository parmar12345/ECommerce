using ECommerce.Application.DTOs.Categories;
using FluentValidation;

namespace ECommerce.Application.Validators.Categories;

public class UpdateCategoryRequestValidator
    : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Category name is required.")
            .MaximumLength(100)
            .WithMessage("Category name cannot exceed 100 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .WithMessage("Category description cannot exceed 500 characters.");
    }
}