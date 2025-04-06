using Application.Abstractions.IServices;
using Application.DTOs.PostDtos;
using Application.DTOs.UpdateDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Filters;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemController(IItemService _itemService) : ControllerBase
{
    [ServiceFilter(typeof(FileValidationFilter))]
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateItem([FromForm] ItemPostDto itemPostDto,CancellationToken ct)
    {
        var result = await _itemService.CreateItemAsync(itemPostDto,ct);
        return result.IsSuccess ? Created("CreatedItem", result.Value) : BadRequest(result.ErrorMessage);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllItems([FromQuery] bool includeImages = false,CancellationToken ct = default)
    {
        var result = await _itemService.GetAllItemsAsync(includeImages,ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
    }

    [HttpGet("category/{categoryId:guid}")]
    public async Task<IActionResult> GetItemsByCategoryId(Guid categoryId, [FromQuery] bool includeImages = false,CancellationToken ct = default)
    {
        var result = await _itemService.GetItemsByCategoryAsync(categoryId, includeImages,ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
    }

    [HttpGet("{itemId:guid}")]
    public async Task<IActionResult> GetItemById(Guid itemId, CancellationToken ct = default)
    {
        var result = await _itemService.GetItemByIdAsync(itemId,ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteItem(Guid id, CancellationToken ct)
    {
        var result = await _itemService.DeleteItemAsync(id,ct);
        return result.IsSuccess ? NoContent() : BadRequest(result.ErrorMessage);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateItem(Guid id, [FromBody] ItemUpdateDto itemUpdateDto, CancellationToken ct)
    {
        var result = await _itemService.UpdateItemAsync(id, itemUpdateDto, ct);
        return result.IsSuccess ? Ok() : BadRequest(result.ErrorMessage);
    }
}