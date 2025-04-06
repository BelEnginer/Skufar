using Application.Abstractions.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CategoryController(ICategoryService _categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllCategories(CancellationToken ct)
    {
        var result = await _categoryService.GetAllCategoriesAsync(ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
    }

    [HttpGet("{categoryId:guid}")]
    public async Task<IActionResult> GetCategoryById(Guid categoryId,CancellationToken ct)
    {
        var result = await _categoryService.GetCategoryByIdAsync(categoryId,ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
    }
}
