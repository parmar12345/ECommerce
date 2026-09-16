namespace ECommerce.Application.DTOs.Products;

public class ProductImageResponse
{
    public Guid Id { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public bool IsPrimary { get; set; }
}