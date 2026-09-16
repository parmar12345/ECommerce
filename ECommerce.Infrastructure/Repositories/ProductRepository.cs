using ECommerce.Application.Common.Models;
using ECommerce.Application.DTOs.Products;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Product> CreateAsync(Product product)
    {
        await _context.Products.AddAsync(product);

        await _context.SaveChangesAsync();

        return product;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products
            .Include(x => x.Category)
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);
    }

    public async Task<PagedResult<Product>> GetAllAsync(
     ProductQueryRequest request)
    {
        var query = _context.Products
            .AsNoTracking()
            .Include(x => x.Category)
            .Include(x => x.Images)
            .Where(x => !x.IsDeleted);

        // Search
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();

            query = query.Where(x =>
                EF.Functions.ILike(
                    x.Name,
                    $"%{search}%") ||

                EF.Functions.ILike(
                    x.Description,
                    $"%{search}%"));
        }

        // Category filter
        if (request.CategoryId.HasValue)
        {
            query = query.Where(x =>
                x.CategoryId == request.CategoryId.Value);
        }

        // Minimum price
        if (request.MinPrice.HasValue)
        {
            query = query.Where(x =>
                x.Price >= request.MinPrice.Value);
        }

        // Maximum price
        if (request.MaxPrice.HasValue)
        {
            query = query.Where(x =>
                x.Price <= request.MaxPrice.Value);
        }

        // Sorting
        query = request.SortBy.ToLowerInvariant() switch
        {
            "name" => request.SortDirection
                .Equals(
                    "asc",
                    StringComparison.OrdinalIgnoreCase)
                ? query
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.Id)
                : query
                    .OrderByDescending(x => x.Name)
                    .ThenBy(x => x.Id),

            "price" => request.SortDirection
                .Equals(
                    "asc",
                    StringComparison.OrdinalIgnoreCase)
                ? query
                    .OrderBy(x => x.Price)
                    .ThenBy(x => x.Id)
                : query
                    .OrderByDescending(x => x.Price)
                    .ThenBy(x => x.Id),

            "createdat" => request.SortDirection
                .Equals(
                    "asc",
                    StringComparison.OrdinalIgnoreCase)
                ? query
                    .OrderBy(x => x.CreatedAt)
                    .ThenBy(x => x.Id)
                : query
                    .OrderByDescending(x => x.CreatedAt)
                    .ThenBy(x => x.Id),

            _ => query
                .OrderByDescending(x => x.CreatedAt)
                .ThenBy(x => x.Id)
        };

        // Total count
        var totalCount = await query.CountAsync();

        // Pagination
        var products = await query
            .Skip(
                (request.PageNumber - 1) *
                request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResult<Product>
        {
            Items = products,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            TotalPages = (int)Math.Ceiling(
                totalCount / (double)request.PageSize)
        };
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        _context.Products.Update(product);

        await _context.SaveChangesAsync();

        return product;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);

        if (product == null)
        {
            return false;
        }

        product.IsDeleted = true;
        product.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }
}