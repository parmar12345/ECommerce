using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<Category> CreateAsync(Category category);

    Task<Category?> GetByIdAsync(Guid id);

    Task<Category?> GetByNameAsync(string name);

    Task<List<Category>> GetAllAsync();

    Task<Category> UpdateAsync(Category category);

    Task<bool> DeleteAsync(Guid id);
}