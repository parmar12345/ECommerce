using Microsoft.AspNetCore.Http;

namespace ECommerce.Application.DTOs.Products;

public class UploadProductImageRequest
{
    public IFormFile File { get; set; } = null!;
}