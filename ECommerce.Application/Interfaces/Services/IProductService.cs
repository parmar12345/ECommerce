using ECommerce.Application.Common.Models;
using ECommerce.Application.DTOs.Products;

namespace ECommerce.Application.Interfaces.Services;

public interface IProductService
{
    Task<ProductResponse> CreateAsync(
        CreateProductRequest request);

    Task<ProductResponse> GetByIdAsync(
        Guid id);

    Task<PagedResult<ProductResponse>> GetAllAsync(
    ProductQueryRequest request);

    Task<ProductResponse> UpdateAsync(
        Guid id,
        UpdateProductRequest request);

    Task DeleteAsync(Guid id);

    Task<ProductImageResponse> UploadImageAsync(
        Guid productId,
        UploadProductImageRequest request);

    Task<List<ProductImageResponse>> GetImagesAsync(
        Guid productId);

    Task SetPrimaryImageAsync(
        Guid productId,
        Guid imageId);

    Task DeleteImageAsync(
        Guid productId,
        Guid imageId);
}