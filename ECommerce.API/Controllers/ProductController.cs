using ECommerce.Application.Common.Models;
using ECommerce.Application.DTOs.Products;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(
        IProductService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductRequest request)
    {
        var product =
            await _productService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id)
    {
        var product =
            await _productService.GetByIdAsync(id);

        return Ok(product);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
      [FromQuery] ProductQueryRequest request)
    {
        var products =
            await _productService.GetAllAsync(request);

        return Ok(products);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProductRequest request)
    {
        var product =
            await _productService.UpdateAsync(
                id,
                request);

        return Ok(product);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id)
    {
        await _productService.DeleteAsync(id);

        return NoContent();
    }

    [HttpPost("{id:guid}/images")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage(
    Guid id,
    [FromForm] UploadProductImageRequest request)
    {
        var image =
            await _productService.UploadImageAsync(
                id,
                request);

        return Ok(image);
    }

    [HttpGet("{id:guid}/images")]
    public async Task<IActionResult> GetImages(
    Guid id)
    {
        var images =
            await _productService.GetImagesAsync(id);

        return Ok(images);
    }

    [HttpPut("{id:guid}/images/{imageId:guid}/primary")]
    public async Task<IActionResult> SetPrimaryImage(
    Guid id,
    Guid imageId)
    {
        await _productService.SetPrimaryImageAsync(
            id,
            imageId);

        return NoContent();
    }

    [HttpDelete("{id:guid}/images/{imageId:guid}")]
    public async Task<IActionResult> DeleteImage(
    Guid id,
    Guid imageId)
    {
        await _productService.DeleteImageAsync(
            id,
            imageId);

        return NoContent();
    }
}