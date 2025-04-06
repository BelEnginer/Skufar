using Application.Abstractions.IServices;
using Application.DTOs.GetDtos;
using Application.DTOs.PostDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ExchangeController(IExchangeService _exchangeService) : ControllerBase
{
    [Authorize]
    [HttpPost("request")]
    public async Task<IActionResult> CreateTradeRequest([FromBody] TradeRequestPostDto tradeRequestPostDto, CancellationToken ct)
    {
        var result = await _exchangeService.RequestExchangeAsync(tradeRequestPostDto, ct);
        return result.IsSuccess ? Created("CreatedRequest", result.Value) : BadRequest(result.ErrorMessage);
    }
    [Authorize]
    [HttpPut("Accept")]
    public async Task<IActionResult> AcceptTradeRequest([FromBody] TradeRequestDto tradeRequestDto, CancellationToken ct)
    {
        var result = await _exchangeService.AcceptExchangeAsync(tradeRequestDto, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
    }
    [Authorize]
    [HttpPut("Reject")]
    public async Task<IActionResult> RejectTradeRequest([FromBody] TradeRequestDto tradeRequestDto, CancellationToken ct)
    {
        var result = await _exchangeService.RejectExchangeAsync(tradeRequestDto, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
    }
    [Authorize]
    [HttpDelete("Cancel")]
    public async Task<IActionResult> CancelTradeRequest([FromBody] TradeRequestDto tradeRequestDto, CancellationToken ct)
    {
        var result = await _exchangeService.CancelExchangeAsync(tradeRequestDto, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
    }
    [Authorize]
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetTradeRequest([FromRoute] Guid userId, CancellationToken ct)
    {
        var result = await _exchangeService.GetUserTradeRequestsAsync(userId, ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
    }
}