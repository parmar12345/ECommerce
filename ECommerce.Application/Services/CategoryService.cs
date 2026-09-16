using ECommerce.Application.DTOs.Categories;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(
        ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryResponse> CreateAsync(
        CreateCategoryRequest request)
    {
        var name = request.Name.Trim();

        var existingCategory =
            await _categoryRepository.GetByNameAsync(name);

        if (existingCategory != null)
        {
            throw new InvalidOperationException(
                "A category with this name already exists.");
        }

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = request.Description.Trim(),
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        var createdCategory =
            await _categoryRepository.CreateAsync(category);

        return MapToResponse(createdCategory);
    }

    public async Task<CategoryResponse> GetByIdAsync(
        Guid id)
    {
        var category =
            await _categoryRepository.GetByIdAsync(id);

        if (category == null)
        {
            throw new KeyNotFoundException(
                "Category not found.");
        }

        return MapToResponse(category);
    }

    public async Task<List<CategoryResponse>> GetAllAsync()
    {
        var categories =
            await _categoryRepository.GetAllAsync();

        return categories
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<CategoryResponse> UpdateAsync(
        Guid id,
        UpdateCategoryRequest request)
    {
        var category =
            await _categoryRepository.GetByIdAsync(id);

        if (category == null)
        {
            throw new KeyNotFoundException(
                "Category not found.");
        }

        var name = request.Name.Trim();

        var existingCategory =
            await _categoryRepository.GetByNameAsync(name);

        if (existingCategory != null &&
            existingCategory.Id != id)
        {
            throw new InvalidOperationException(
                "A category with this name already exists.");
        }

        category.Name = name;
        category.Description = request.Description.Trim();
        category.UpdatedAt = DateTime.UtcNow;

        var updatedCategory =
            await _categoryRepository.UpdateAsync(category);

        return MapToResponse(updatedCategory);
    }

    public async Task DeleteAsync(Guid id)
    {
        var deleted =
            await _categoryRepository.DeleteAsync(id);

        if (!deleted)
        {
            throw new KeyNotFoundException(
                "Category not found.");
        }
    }

    private static CategoryResponse MapToResponse(
        Category category)
    {
        return new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }
}