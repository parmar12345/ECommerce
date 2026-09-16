using ECommerce.Application.DTOs.Products;
using FluentValidation;

namespace ECommerce.Application.Validators.Products;

public class CreateProductRequestValidator
    : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Product name is required.")
            .MaximumLength(200)
            .WithMessage("Product name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Product description cannot exceed 2000 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Product price must be greater than 0.");

        RuleFor(x => x.StockQuantity)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Stock quantity cannot be negative.");

        RuleFor(x => x.CategoryId)
            .NotEqual(Guid.Empty)
            .WithMessage("Category is required.");
    }
}