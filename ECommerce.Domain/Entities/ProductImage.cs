namespace ECommerce.Domain.Entities;

public class ProductImage
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public int DisplayOrder { get; set; }

    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; set; }

    // Navigation property
    public Product Product { get; set; } = null!;
}