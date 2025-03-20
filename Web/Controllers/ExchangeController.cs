using Application.DTOs.GetDtos;
using Application.DTOs.PostDtos;
using Application.IServices;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;

namespace Web.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ExchangeController(IExchangeService exchangeService) : ControllerBase
{
    //[Authorize]
    [HttpPost("request")]
    public async Task<IActionResult> CreateTradeRequest([FromBody] TradeRequestPostDto tradeRequestPostDto, CancellationToken ct)
    {
        var result = await exchangeService.RequestExchangeAsync(tradeRequestPostDto, ct);
        var errorResponse = this.HandleError(result);
        return errorResponse != null ? errorResponse : Created("CreatedRequest", result.Value);
    }
    //[Authorize]
    [HttpPut("Accept")]
    public async Task<IActionResult> AcceptTradeRequest([FromBody] TradeRequestDto tradeRequestDto, CancellationToken ct)
    {
        var result = await exchangeService.AcceptExchangeAsync(tradeRequestDto, ct);
        var errorResponse = this.HandleError(result);
        return errorResponse != null ? errorResponse : Ok(result.Value);
    }
    //[Authorize]
    [HttpPut("Reject")]
    public async Task<IActionResult> RejectTradeRequest([FromBody] TradeRequestDto tradeRequestDto, CancellationToken ct)
    {
        var result = await exchangeService.RejectExchangeAsync(tradeRequestDto, ct);
        var errorResponse = this.HandleError(result);
        return errorResponse != null ? errorResponse : Ok(result.Value);
    }
    //[Authorize]
    [HttpDelete("Cancel")]
    public async Task<IActionResult> CancelTradeRequest([FromBody] TradeRequestDto tradeRequestDto, CancellationToken ct)
    {
        var result = await exchangeService.CancelExchangeAsync(tradeRequestDto, ct);
        var errorResponse = this.HandleError(result);
        return errorResponse != null ? errorResponse : Ok(result.Value);
    }
    //[Authorize]
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetTradeRequest([FromRoute] Guid userId, CancellationToken ct)
    {
        var result = await exchangeService.GetUserTradeRequestsAsync(userId, ct);
        var errorResponse = this.HandleError(result);
        return errorResponse != null ? errorResponse : Ok(result.Value);
    }
}