using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class ProductImageRepository : IProductImageRepository
{
    private readonly ApplicationDbContext _context;

    public ProductImageRepository(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductImage> CreateAsync(
        ProductImage image)
    {
        await _context.ProductImages.AddAsync(image);

        await _context.SaveChangesAsync();

        return image;
    }

    public async Task<List<ProductImage>> GetByProductIdAsync(
        Guid productId)
    {
        return await _context.ProductImages
            .Where(x => x.ProductId == productId)
            .OrderBy(x => x.DisplayOrder)
            .ToListAsync();
    }

    public async Task<ProductImage?> GetByIdAsync(Guid id)
    {
        return await _context.ProductImages
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ProductImage?> GetPrimaryImageAsync(
        Guid productId)
    {
        return await _context.ProductImages
            .FirstOrDefaultAsync(x =>
                x.ProductId == productId &&
                x.IsPrimary);
    }

    public async Task SetPrimaryAsync(
        Guid productId,
        Guid imageId)
    {
        var images = await _context.ProductImages
            .Where(x => x.ProductId == productId)
            .ToListAsync();

        foreach (var image in images)
        {
            image.IsPrimary = image.Id == imageId;
        }

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ProductImage image)
    {
        _context.ProductImages.Remove(image);

        await _context.SaveChangesAsync();
    }

    public async Task<ProductImage> UpdateAsync(
    ProductImage image)
    {
        _context.ProductImages.Update(image);

        await _context.SaveChangesAsync();

        return image;
    }
}