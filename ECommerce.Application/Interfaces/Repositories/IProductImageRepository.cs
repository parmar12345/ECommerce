using ECommerce.Domain.Entities;

public interface IProductImageRepository
{
    Task<ProductImage> CreateAsync(ProductImage image);

    Task<List<ProductImage>> GetByProductIdAsync(Guid productId);

    Task<ProductImage?> GetByIdAsync(Guid id);

    Task<ProductImage> UpdateAsync(ProductImage image);

    Task DeleteAsync(ProductImage image);

    Task<ProductImage?> GetPrimaryImageAsync(Guid productId);

    Task SetPrimaryAsync(Guid productId, Guid imageId);
}