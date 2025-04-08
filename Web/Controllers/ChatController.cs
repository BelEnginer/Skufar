using Application.Abstractions.IServices;
using Application.DTOs.PostDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;

namespace Web.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ChatController(IChatService _chatService) : ControllerBase
{
  [Authorize]
  [HttpGet("user/{userId:guid}/chats")]
  public async Task<IActionResult> GetUserChatsAsync(Guid userId, CancellationToken ct)
  {
    var result = await _chatService.GetUserChatsAsync(userId, ct);
    return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
  }
  
  [Authorize]
  [HttpGet("{chatId:guid}/messages")]
  public async Task<IActionResult> GetMessagesInChatAsync(Guid chatId, CancellationToken ct)
  {
    var result = await _chatService.GetMessagesByChatIdAsync(chatId, ct);
    return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
  }
  
  [Authorize]
  [HttpPost("send-message")]
  public async Task<IActionResult> SendMessageAsync([FromBody] MessagePostDto messagePostDto, CancellationToken ct)
  {
    var userId = HttpContext.GetUserId();
    var result = await _chatService.SendMessageAsync(messagePostDto,userId, ct);
    return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
  }
  
  //[Authorize]
  [HttpDelete("{chatId:guid}/messages/{messageId:guid}")]
  public async Task<IActionResult> DeleteMessageAsync(Guid chatId, Guid messageId, CancellationToken ct)
  {
    var result = await _chatService.DeleteMessageAsync(chatId, messageId, ct);
    return result.IsSuccess ? NoContent() : BadRequest(result.ErrorMessage);
  }

  [HttpDelete("{chatId:guid}")]
  public async Task<IActionResult> DeleteChatAsync(Guid chatId, CancellationToken ct)
  {
    var result = await _chatService.DeleteChatAsync(chatId, ct);
    return result.IsSuccess ? NoContent() : BadRequest(result.ErrorMessage);
  }
}
