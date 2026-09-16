using ECommerce.Application.DTOs.Categories;

namespace ECommerce.Application.Interfaces.Services;

public interface ICategoryService
{
    Task<CategoryResponse> CreateAsync(
        CreateCategoryRequest request);

    Task<CategoryResponse> GetByIdAsync(
        Guid id);

    Task<List<CategoryResponse>> GetAllAsync();

    Task<CategoryResponse> UpdateAsync(
        Guid id,
        UpdateCategoryRequest request);

    Task DeleteAsync(Guid id);
}