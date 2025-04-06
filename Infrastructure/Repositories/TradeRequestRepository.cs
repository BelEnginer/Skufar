
using Application.Abstractions.IRepositories;
using Domain.Models;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class TradeRequestRepository(ApplicationDbContext context) : BaseRepository<TradeRequest>(context), ITradeRequestRepository
{
    public async Task<TradeRequest?> GetTradeRequestByIdAsync(Guid id,CancellationToken ct) => 
        await GetByFilter(i => i.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id,ct);
    

    public async Task<List<TradeRequest?>> GetTradeRequestsByUserIdAsync(Guid userId, CancellationToken ct) =>
        (await GetByFilter(i => i.SenderId == userId)
            .AsNoTracking()
            .ToListAsync(ct))!;

    public async Task CreateTradeRequestAsync(TradeRequest tradeRequest,CancellationToken ct)
    {
        tradeRequest.Id = Guid.NewGuid();
        tradeRequest.CreatedAt = DateTime.UtcNow;
        await Context.TradeRequests.AddAsync(tradeRequest,ct);
        await Context.SaveChangesAsync(ct);
    }

    public async Task<bool> ExistsAsync(Guid itemOfferedId, Guid itemRequestedId, Guid senderId, CancellationToken ct) =>
        await context.TradeRequests
            .AnyAsync(i => i.ItemOfferedId == itemOfferedId 
                           && i.ItemRequestedId == itemRequestedId 
                           && i.SenderId == senderId, ct);

    public async Task DeleteTradeRequestAsync(TradeRequest tradeRequest,CancellationToken ct)
    {
        Delete(tradeRequest);
        await Context.SaveChangesAsync(ct);
    }
}