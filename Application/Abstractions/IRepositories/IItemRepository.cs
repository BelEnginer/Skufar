using Domain.Entites;

namespace Application.Abstractions.IRepositories;

public interface IItemRepository
{
    Task <List<Item?>> GetItemsByIdsAsync(List<Guid> itemIds,CancellationToken ct);
    Task <List<Item>> GetAllItemsAsync(CancellationToken ct);
    Task<List<Item>> GetItemsByCategoryAsync(Guid categoryId, CancellationToken ct);
    Task<Item?> GetItemByIdAsync(Guid itemId, CancellationToken ct);
    Task<bool> IsItemOwnedByUser(Guid itemId, Guid userId, CancellationToken ct);
    Task ChangeOwnerOfItemAsync(Guid item1Id,Guid item2Id,CancellationToken ct);
    Task<List<Item>> GetItemsByUserAsync(Guid userId, CancellationToken ct);
    Task CreateItemAsync(Item item, CancellationToken ct);
    void DeleteItem(Item item);
    Task UpdateItemAsync(Item item, CancellationToken ct);
    
}
