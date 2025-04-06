using Application.Common;
using Application.DTOs.GetDtos;
using Application.DTOs.PostDtos;

namespace Application.Abstractions.IServices;

public interface IChatService
{
    Task<Result<List<ChatDto>>> GetUserChatsAsync(Guid userId, CancellationToken ct);
    Task<Result<List<MessageDto>>> GetMessagesByChatIdAsync(Guid chatId, CancellationToken ct/*, int pageSize = 20, int page = 1*/);
    Task<Result<MessageDto>> SendMessageAsync(MessagePostDto messagePostDto, CancellationToken ct);
    Task<Result<Unit>> DeleteChatAsync(Guid chatId, CancellationToken ct);
    Task<Result<Unit>> DeleteMessageAsync(Guid chatId,Guid messageId, CancellationToken ct);
}

