using Domain.Models;

namespace Application.Abstractions.IRepositories;

public interface ITradeRequestRepository
{
    Task<TradeRequest?> GetTradeRequestByIdAsync(Guid tradeRequestId, CancellationToken ct);
    Task<List<TradeRequest>> GetTradeRequestsByUserIdAsync(Guid senderId, CancellationToken ct);
    Task CreateTradeRequestAsync(TradeRequest tradeRequest, CancellationToken ct);
    Task<bool> ExistsAsync(Guid iteOfferedId,Guid itemRequestedId,Guid senderId, CancellationToken ct);
    Task DeleteTradeRequestAsync(TradeRequest tradeRequest,CancellationToken ct);
}