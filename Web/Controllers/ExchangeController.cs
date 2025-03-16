using Application.DTOs.GetDtos;
using Application.DTOs.PostDtos;
using Application.IServices;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

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
        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(result.ErrorMessage),
                ErrorType.Conflict => Conflict(result.ErrorMessage),
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error")
            };
        }
        return Created("CreatedRequest", result.Value);
    }
    //[Authorize]
    [HttpPut("Accept")]
    public async Task<IActionResult> AcceptTradeRequest([FromBody] TradeRequestDto tradeRequestDto, CancellationToken ct)
    {
        var result = await exchangeService.AcceptExchangeAsync(tradeRequestDto, ct);
        return result.ErrorType switch
        {
            ErrorType.NotFound => NotFound(result.ErrorMessage),
            ErrorType.Conflict => Conflict(result.ErrorMessage),
            _ => StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error")
        };
    }
    //[Authorize]
    [HttpPut("Reject")]
    public async Task<IActionResult> RejectTradeRequest([FromBody] TradeRequestDto tradeRequestDto, CancellationToken ct)
    {
        var result = await exchangeService.RejectExchangeAsync(tradeRequestDto, ct);
        return result.ErrorType switch
        {
            ErrorType.NotFound => NotFound(result.ErrorMessage),
            ErrorType.Conflict => Conflict(result.ErrorMessage),
            _ => StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error")
        };
    }
    //[Authorize]
    [HttpDelete("Cancel")]
    public async Task<IActionResult> CancelTradeRequest([FromBody] TradeRequestDto tradeRequestDto, CancellationToken ct)
    {
        var result = await exchangeService.CancelExchangeAsync(tradeRequestDto, ct);
        return result.ErrorType switch
        {
            ErrorType.NotFound => NotFound(result.ErrorMessage),
            ErrorType.Conflict => Conflict(result.ErrorMessage),
            _ => StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error")
        };
    }
    //[Authorize]
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetTradeRequest([FromRoute] Guid userId, CancellationToken ct)
    {
        var result = await exchangeService.GetUserTradeRequestsAsync(userId,ct);
        if (!result.IsSuccess)
        {
            return result.ErrorType switch
            {
                ErrorType.NotFound => NotFound(result.ErrorMessage),
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Unknown Error")
            };
        }
        return Ok(result.Value);
    }
}