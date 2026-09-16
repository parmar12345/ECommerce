using ECommerce.Application.Common.Models;
using ECommerce.Application.DTOs.Products;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<Product> CreateAsync(Product product);

    Task<Product?> GetByIdAsync(Guid id);

    Task<PagedResult<Product>> GetAllAsync(
        ProductQueryRequest request);

    Task<Product> UpdateAsync(Product product);

    Task<bool> DeleteAsync(Guid id);
}