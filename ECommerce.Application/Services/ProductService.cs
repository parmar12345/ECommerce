using ECommerce.Application.Common.Models;
using ECommerce.Application.DTOs.Products;
using ECommerce.Application.Interfaces.Repositories;
using ECommerce.Application.Interfaces.Services;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductImageRepository _productImageRepository;
    private readonly IFileStorageService _fileStorageService;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IProductImageRepository productImageRepository,
        IFileStorageService fileStorageService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _productImageRepository = productImageRepository;
        _fileStorageService = fileStorageService;
    }

    // =========================================================
    // Product CRUD
    // =========================================================

    public async Task<ProductResponse> CreateAsync(
        CreateProductRequest request)
    {
        var category =
            await _categoryRepository.GetByIdAsync(
                request.CategoryId);

        if (category == null)
        {
            throw new KeyNotFoundException(
                "Category not found.");
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description.Trim(),
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        var createdProduct =
            await _productRepository.CreateAsync(product);

        return MapToResponse(createdProduct);
    }

    public async Task<ProductResponse> GetByIdAsync(
        Guid id)
    {
        var product =
            await _productRepository.GetByIdAsync(id);

        if (product == null)
        {
            throw new KeyNotFoundException(
                "Product not found.");
        }

        return MapToResponse(product);
    }

    public async Task<PagedResult<ProductResponse>> GetAllAsync(
      ProductQueryRequest request)
    {
        var result =
            await _productRepository.GetAllAsync(request);

        return new PagedResult<ProductResponse>
        {
            Items = result.Items
                .Select(MapToResponse)
                .ToList(),

            PageNumber = result.PageNumber,

            PageSize = result.PageSize,

            TotalCount = result.TotalCount,

            TotalPages = result.TotalPages
        };
    }

    public async Task<ProductResponse> UpdateAsync(
        Guid id,
        UpdateProductRequest request)
    {
        var product =
            await _productRepository.GetByIdAsync(id);

        if (product == null)
        {
            throw new KeyNotFoundException(
                "Product not found.");
        }

        var category =
            await _categoryRepository.GetByIdAsync(
                request.CategoryId);

        if (category == null)
        {
            throw new KeyNotFoundException(
                "Category not found.");
        }

        product.Name = request.Name.Trim();
        product.Description = request.Description.Trim();
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;
        product.CategoryId = request.CategoryId;
        product.UpdatedAt = DateTime.UtcNow;

        var updatedProduct =
            await _productRepository.UpdateAsync(product);

        return MapToResponse(updatedProduct);
    }

    public async Task DeleteAsync(Guid id)
    {
        var deleted =
            await _productRepository.DeleteAsync(id);

        if (!deleted)
        {
            throw new KeyNotFoundException(
                "Product not found.");
        }
    }

    // =========================================================
    // Product Images
    // =========================================================

    public async Task<ProductImageResponse> UploadImageAsync(
        Guid productId,
        UploadProductImageRequest request)
    {
        var product =
            await _productRepository.GetByIdAsync(productId);

        if (product == null)
        {
            throw new KeyNotFoundException(
                "Product not found.");
        }

        if (request.File == null ||
            request.File.Length == 0)
        {
            throw new InvalidOperationException(
                "Image file is required.");
        }

        var allowedExtensions = new[]
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        var extension =
            Path.GetExtension(request.File.FileName)
                .ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException(
                "Only JPG, JPEG, PNG and WebP images are allowed.");
        }

        const long maxFileSize = 5 * 1024 * 1024;

        if (request.File.Length > maxFileSize)
        {
            throw new InvalidOperationException(
                "Image size cannot exceed 5 MB.");
        }

        var existingImages =
            await _productImageRepository
                .GetByProductIdAsync(productId);

        var displayOrder =
            existingImages.Count + 1;

        var isPrimary =
            existingImages.Count == 0;

        await using var stream =
            request.File.OpenReadStream();

        var imageUrl =
            await _fileStorageService.SaveAsync(
                stream,
                request.File.FileName,
                request.File.ContentType);

        var image = new ProductImage
        {
            Id = Guid.NewGuid(),
            ProductId = productId,
            ImageUrl = imageUrl,
            DisplayOrder = displayOrder,
            IsPrimary = isPrimary,
            CreatedAt = DateTime.UtcNow
        };

        var createdImage =
            await _productImageRepository
                .CreateAsync(image);

        return MapToImageResponse(createdImage);
    }

    public async Task<List<ProductImageResponse>> GetImagesAsync(
        Guid productId)
    {
        var product =
            await _productRepository.GetByIdAsync(productId);

        if (product == null)
        {
            throw new KeyNotFoundException(
                "Product not found.");
        }

        var images =
            await _productImageRepository
                .GetByProductIdAsync(productId);

        return images
            .Select(MapToImageResponse)
            .ToList();
    }

    public async Task SetPrimaryImageAsync(
     Guid productId,
     Guid imageId)
    {
        // 1. Check product
        var product = await _productRepository
            .GetByIdAsync(productId);

        if (product == null)
        {
            throw new KeyNotFoundException(
                "Product not found.");
        }

        // 2. Check image
        var image = await _productImageRepository
            .GetByIdAsync(imageId);

        if (image == null)
        {
            throw new KeyNotFoundException(
                "Product image not found.");
        }

        // 3. Make sure image belongs to this product
        if (image.ProductId != productId)
        {
            throw new InvalidOperationException(
                "The image does not belong to this product.");
        }

        // 4. Set primary
        await _productImageRepository
            .SetPrimaryAsync(productId, imageId);
    }

    public async Task DeleteImageAsync(
        Guid productId,
        Guid imageId)
    {
        var product =
            await _productRepository.GetByIdAsync(productId);

        if (product == null)
        {
            throw new KeyNotFoundException(
                "Product not found.");
        }

        var image =
            await _productImageRepository
                .GetByIdAsync(imageId);

        if (image == null ||
            image.ProductId != productId)
        {
            throw new KeyNotFoundException(
                "Product image not found.");
        }

        var wasPrimary = image.IsPrimary;

        // Delete physical file first
        await _fileStorageService
            .DeleteAsync(image.ImageUrl);

        // Delete database record
        await _productImageRepository
            .DeleteAsync(image);

        // If primary image was deleted,
        // make the first remaining image primary.
        if (wasPrimary)
        {
            var remainingImages =
                await _productImageRepository
                    .GetByProductIdAsync(productId);

            var nextPrimaryImage =
                remainingImages
                    .OrderBy(x => x.DisplayOrder)
                    .FirstOrDefault();

            if (nextPrimaryImage != null)
            {
                nextPrimaryImage.IsPrimary = true;

                await _productImageRepository
                    .UpdateAsync(nextPrimaryImage);
            }
        }
    }

    // =========================================================
    // Mapping
    // =========================================================

    private static ProductResponse MapToResponse(
        Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            CategoryId = product.CategoryId,

            CategoryName =
                product.Category?.Name ?? string.Empty,

            Images = product.Images
                .OrderBy(x => x.DisplayOrder)
                .Select(MapToImageResponse)
                .ToList()
        };
    }

    private static ProductImageResponse MapToImageResponse(
        ProductImage image)
    {
        return new ProductImageResponse
        {
            Id = image.Id,
            ImageUrl = image.ImageUrl,
            DisplayOrder = image.DisplayOrder,
            IsPrimary = image.IsPrimary
        };
    }


}