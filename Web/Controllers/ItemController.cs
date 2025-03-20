using Application.DTOs.PostDtos;
using Application.DTOs.UpdateDtos;
using Application.IServices;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;
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
        var errorResponse = this.HandleError(result);
        return errorResponse != null ? errorResponse : Created("CreatedItem", result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllItems([FromQuery] bool includeImages = false,CancellationToken ct = default)
    {
        var result = await itemService.GetAllItemsAsync(includeImages,ct);
        var errorResponse = this.HandleError(result);
        return errorResponse != null ? errorResponse : Ok(result.Value);
    }

    [HttpGet("category/{categoryId:guid}")]
    public async Task<IActionResult> GetItemsByCategoryId(Guid categoryId, [FromQuery] bool includeImages = false,CancellationToken ct = default)
    {
        var result = await itemService.GetItemsByCategoryAsync(categoryId, includeImages,ct);
        var errorResponse = this.HandleError(result);
        return errorResponse != null ? errorResponse : Ok(result.Value);
    }

    [HttpGet("{itemId:guid}")]
    public async Task<IActionResult> GetItemById(Guid itemId, CancellationToken ct = default)
    {
        var result = await itemService.GetItemByIdAsync(itemId,ct);
        var errorResponse = this.HandleError(result);
        return errorResponse != null ? errorResponse : Ok(result.Value);
    }

    //[Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteItem(Guid id, CancellationToken ct)
    {
        var result = await itemService.DeleteItemAsync(id,ct);
        var errorResponse = this.HandleError(result);
        return errorResponse != null ? errorResponse : NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateItem(Guid id, [FromBody] ItemUpdateDto itemUpdateDto, CancellationToken ct)
    {
        var result = await itemService.UpdateItemAsync(id, itemUpdateDto, ct);
        var errorResponse = this.HandleError(result);
        return errorResponse != null ? errorResponse : Ok();
    }
}