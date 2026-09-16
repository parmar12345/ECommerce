using ECommerce.Application.DTOs.Categories;
using ECommerce.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(
        ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryRequest request)
    {
        var category =
            await _categoryService.CreateAsync(request);

        return CreatedAtAction(
            nameof(GetById),
            new { id = category.Id },
            category);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id)
    {
        var category =
            await _categoryService.GetByIdAsync(id);

        return Ok(category);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories =
            await _categoryService.GetAllAsync();

        return Ok(categories);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCategoryRequest request)
    {
        var category =
            await _categoryService.UpdateAsync(
                id,
                request);

        return Ok(category);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id)
    {
        await _categoryService.DeleteAsync(id);

        return NoContent();
    }
}