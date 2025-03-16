
using Application.Abstractions.IRepositories;
using Domain.Entites;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ItemRepository(ApplicationDbContext context) : BaseRepository<Item>(context), IItemRepository
{
    private static IQueryable<Item> IncludeAllRelations(IQueryable<Item> query)
    {
        return query
            .Include(c => c.Category)
            .Include(i => i.Images)
            .Include(o => o.Owner);
    }
    public async Task<Item?> GetItemByIdAsync(Guid id,CancellationToken ct) => 
        await IncludeAllRelations(GetByFilter(c => c.Id == id))
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);
    
    public async Task<List<Item?>> GetItemsByIdsAsync(List<Guid> itemsId,CancellationToken ct) => 
        (await IncludeAllRelations(context.Items)
        .Where(i => itemsId.Contains(i.Id))
        .AsNoTracking()
        .ToListAsync(ct))!;

    public async Task<bool> IsItemOwnedByUser(Guid itemId, Guid userId, CancellationToken ct) => 
        await context.Items.AnyAsync(i => i.Id == itemId && i.Owner.Id == userId, ct);

    public async Task ChangeOwnerOfItemAsync(Guid item1Id, Guid item2Id, CancellationToken ct)
    {
        var items = await IncludeAllRelations(context.Items)
            .Where(i => i.Id == item1Id && i.Owner.Id == item2Id)
            .ToListAsync(ct);
        (items[0].Owner.Id, items[1].Owner.Id) = (items[1].Owner.Id, items[0].Owner.Id);
        await context.SaveChangesAsync(ct);
    }

    public async Task<List<Item>> GetAllItemsAsync(CancellationToken ct) => 
        await IncludeAllRelations(GetAll())
            .AsNoTracking()
            .ToListAsync(ct);
    
    public async Task<List<Item>> GetItemsByUserAsync(Guid userId, CancellationToken ct) => 
        await IncludeAllRelations(GetByFilter(u => u.Owner.Id == userId))
            .AsNoTracking()
            .ToListAsync(ct);
    public async Task CreateItemAsync(Item item,CancellationToken ct)
    {
        item.Id = Guid.NewGuid();
        item.CreatedAt = DateTime.UtcNow;
        await Context.Items.AddAsync(item,ct);
        await Context.SaveChangesAsync(ct);
    }
    
    public async Task<List<Item>> GetItemsByCategoryAsync(Guid categoryId,CancellationToken ct) => 
        await IncludeAllRelations(GetByFilter(m => m.Category.Id == categoryId))
            .AsNoTracking()
            .ToListAsync(ct);
    
    public void DeleteItem(Item item)
    {
        Delete(item);
        Context.SaveChanges();
    }
    
    public async Task UpdateItemAsync(Item item,CancellationToken ct)
    => await Context.SaveChangesAsync(ct);
    
    
}