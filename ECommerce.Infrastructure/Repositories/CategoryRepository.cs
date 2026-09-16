using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Category> CreateAsync(Category category)
    {
        await _context.Categories.AddAsync(category);

        await _context.SaveChangesAsync();

        return category;
    }

    public async Task<Category?> GetByIdAsync(Guid id)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);
    }

    public async Task<Category?> GetByNameAsync(string name)
    {
        return await _context.Categories
            .FirstOrDefaultAsync(x =>
                x.Name == name &&
                !x.IsDeleted);
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _context.Categories
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Name)
            .ToListAsync();
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        _context.Categories.Update(category);

        await _context.SaveChangesAsync();

        return category;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);

        if (category == null)
        {
            return false;
        }

        category.IsDeleted = true;
        category.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }
}