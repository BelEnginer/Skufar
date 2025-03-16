using Application.Abstractions.IServices;
using Application.IServices;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;
[ApiController]
[Route("api/[controller]")]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllCategories(CancellationToken ct)
    {
        var result = await categoryService.GetAllCategoriesAsync(ct);
        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error")
            };
        }
        return Ok(result.Value);
    }

    [HttpGet("{categoryId:guid}")]
    public async Task<IActionResult> GetCategoryById(Guid categoryId,CancellationToken ct)
    {
        var result = await categoryService.GetCategoryByIdAsync(categoryId,ct);
        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(result.ErrorMessage),
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error")
            };
        }
        return Ok(result.Value);
    }
}
