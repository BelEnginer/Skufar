using Application.DTOs.PostDtos;
using Application.DTOs.UpdateDtos;
using Application.IServices;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Web.Filters;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemController(IItemService itemService) : ControllerBase
{
    [ServiceFilter(typeof(FileValidationFilter))]
    //[Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateItem([FromForm] ItemPostDto itemPostDto,CancellationToken ct)
    {
        var result = await itemService.CreateItemAsync(itemPostDto,ct);
        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.Unauthorized => Unauthorized(result.ErrorMessage),
                ErrorType.NotFound => NotFound(result.ErrorMessage),
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error")
            };
        }
        return Created("CreatedItem", result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllItems([FromQuery] bool includeImages = false,CancellationToken ct = default)
    {
        var result = await itemService.GetAllItemsAsync(includeImages,ct);
        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error")
            };
        }
        return Ok(result.Value);
    }

    [HttpGet("category/{categoryId:guid}")]
    public async Task<ActionResult> GetItemsByCategoryId(Guid categoryId, [FromQuery] bool includeImages = false,CancellationToken ct = default)
    {
        var result = await itemService.GetItemsByCategoryAsync(categoryId, includeImages,ct);
        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error")
            };
        }
        return Ok(result.Value);
    }

    [HttpGet("{itemId:guid}")]
    public async Task<IActionResult> GetItemById(Guid itemId, CancellationToken ct = default)
    {
        var result = await itemService.GetItemByIdAsync(itemId,ct);
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

    //[Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteItem(Guid id, CancellationToken ct)
    {
        var result = await itemService.DeleteItemAsync(id,ct);
        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(result.ErrorMessage),
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error")
            };
        }
        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateItem(Guid id, [FromBody] ItemUpdateDto itemUpdateDto,CancellationToken ct)
    {
        var result = await itemService.UpdateItemAsync(id, itemUpdateDto,ct);
        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(result.ErrorMessage),
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error")
            };
        }
        return Ok();
    }
}
