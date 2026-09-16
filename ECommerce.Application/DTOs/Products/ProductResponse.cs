namespace ECommerce.Application.DTOs.Products;

public class ProductResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public Guid CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public List<ProductImageResponse> Images { get; set; }
        = new();
}