using ECommerce.Application.DTOs.Products;
using FluentValidation;

namespace ECommerce.Application.Validators.Products;

public class ProductQueryRequestValidator
    : AbstractValidator<ProductQueryRequest>
{
    public ProductQueryRequestValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage(
                "Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage(
                "Page size must be between 1 and 100.");

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinPrice.HasValue)
            .WithMessage(
                "Minimum price cannot be negative.");

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxPrice.HasValue)
            .WithMessage(
                "Maximum price cannot be negative.");

        RuleFor(x => x)
            .Must(x =>
                !x.MinPrice.HasValue ||
                !x.MaxPrice.HasValue ||
                x.MinPrice <= x.MaxPrice)
            .WithMessage(
                "Minimum price cannot be greater than maximum price.");

        RuleFor(x => x.SortBy)
     .Must(x =>
         new[] { "createdAt", "name", "price" }
             .Contains(x, StringComparer.OrdinalIgnoreCase))
     .WithMessage(
         "SortBy must be one of: createdAt, name, price.");

        RuleFor(x => x.SortDirection)
            .Must(x =>
                x.Equals("asc", StringComparison.OrdinalIgnoreCase) ||
                x.Equals("desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage(
                "SortDirection must be either asc or desc.");
    }
}