namespace ECommerce.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public Guid CategoryId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsDeleted { get; set; }

    // Navigation properties
    public Category Category { get; set; } = null!;

    public ICollection<ProductImage> Images { get; set; }
        = new List<ProductImage>();
}