using Application.Common;
using Application.DTOs.GetDtos;
using Application.DTOs.PostDtos;
using Application.DTOs.UpdateDtos;

namespace Application.IServices;

public interface IItemService
{
     Task<Result<List<ItemDto>>> GetAllItemsAsync(bool includeImages,CancellationToken ct);
     Task<Result<ItemDto>> GetItemByIdAsync(Guid id,CancellationToken ct);
     Task<Result<ItemDto>> CreateItemAsync(ItemPostDto itemPostDto,CancellationToken ct);
     Task<Result<List<ItemDto>>> GetItemsByCategoryAsync(Guid categoryId,bool includeImages,CancellationToken ct);
     Task<Result<Unit>> UpdateItemAsync(Guid idItemToUpdate,ItemUpdateDto itemUpdateDto,CancellationToken ct);
     Task<Result<Unit>> DeleteItemAsync(Guid idItemToDelete,CancellationToken ct);
}