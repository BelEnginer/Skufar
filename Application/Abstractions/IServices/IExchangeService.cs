using Application.Common;
using Application.DTOs.GetDtos;
using Application.DTOs.PostDtos;

namespace Application.Abstractions.IServices;

public interface IExchangeService
{
    Task<Result<Unit>> RequestExchangeAsync(TradeRequestPostDto tradeRequestPostDto,CancellationToken ct);
    Task<Result<Unit>> AcceptExchangeAsync(TradeRequestDto tradeRequestUpdateDto, CancellationToken ct);
    Task<Result<Unit>> RejectExchangeAsync(TradeRequestDto tradeRequestUpdateDto, CancellationToken ct);
    Task<Result<Unit>> CancelExchangeAsync(TradeRequestDto tradeRequestUpdateDto, CancellationToken ct);
    Task<Result<List<TradeRequestDto>>> GetUserTradeRequestsAsync(Guid userId, CancellationToken ct);
    //Task<Result<ExchangeStatus>> GetExchangeStatusAsync(Guid requestId)
}