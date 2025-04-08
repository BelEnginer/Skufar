
using Application.Abstractions.IRepositories;
using Domain.Models;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

internal sealed class ItemRepository(ApplicationDbContext context) : BaseRepository<Item>(context), IItemRepository
{
    public async Task<Item?> GetItemByIdAsync(Guid id, CancellationToken ct) => 
        await Query
            .Include(i => i.Category)
            .Include(i => i.Images)
            .Include(i => i.Owner)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<List<Item>> GetItemsByIdsAsync(List<Guid> itemsId, CancellationToken ct) => 
        await Query
            .Include(i => i.Category)
            .Include(i => i.Images)
            .Include(i => i.Owner)
            .Where(i => itemsId.Contains(i.Id))
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<bool> IsItemOwnedByUser(Guid itemId, Guid userId, CancellationToken ct) =>
        await Query.AnyAsync(i => i.Id == itemId && i.Owner.Id == userId, ct);

    public async Task ChangeOwnerOfItemAsync(Guid item1Id, Guid item2Id, CancellationToken ct)
    {
        var items = await Query
            .Include(i => i.Owner)
            .Where(i => i.Id == item1Id || i.Id == item2Id)
            .ToListAsync(ct);
        (items[0].Owner.Id, items[1].Owner.Id) = (items[1].Owner.Id, items[0].Owner.Id);
        await context.SaveChangesAsync(ct);
    }

    public async Task<List<Item>> GetAllItemsAsync(CancellationToken ct) =>
        await Query
            .Include(i => i.Category)
            .Include(i => i.Images)
            .Include(i => i.Owner)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<List<Item>> GetItemsByUserAsync(Guid userId, CancellationToken ct) =>
        await Query
            .Include(i => i.Category)
            .Include(i => i.Images)
            .Include(i => i.Owner)
            .Where(i => i.Owner.Id == userId)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<List<Item>> GetItemsByCategoryAsync(Guid categoryId, CancellationToken ct) =>
        await Query
            .Include(i => i.Category)
            .Include(i => i.Images)
            .Include(i => i.Owner)
            .Where(i => i.Category.Id == categoryId)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task CreateItemAsync(Item item, CancellationToken ct)
    {
        item.Id = Guid.NewGuid();
        item.CreatedAt = DateTime.UtcNow;
        await Context.Items.AddAsync(item, ct);
        await Context.SaveChangesAsync(ct);
    }

    public async Task DeleteItemAsync(Item item, CancellationToken ct)
    {
        Delete(item);
        await Context.SaveChangesAsync(ct);
    }

    public async Task UpdateItemAsync(Item item, CancellationToken ct)
    {
        Update(item);
        await Context.SaveChangesAsync(ct);
    }
}
