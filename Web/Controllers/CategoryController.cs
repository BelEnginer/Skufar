using Application.Abstractions.IServices;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;

namespace Web.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllCategories(CancellationToken ct)
    {
        var result = await categoryService.GetAllCategoriesAsync(ct);
        var errorResponse = this.HandleError(result);
        return errorResponse != null ? errorResponse : Ok(result.Value);
    }

    [HttpGet("{categoryId:guid}")]
    public async Task<IActionResult> GetCategoryById(Guid categoryId,CancellationToken ct)
    {
        var result = await categoryService.GetCategoryByIdAsync(categoryId,ct);
        var errorResponse = this.HandleError(result);
        return errorResponse != null ? errorResponse : Ok(result.Value);
    }
}
