using Application.DTOs.GetDtos;
using Application.DTOs.PostDtos;

namespace Application.Abstractions.IServices;

public interface IChatCacheService
{
    Task<List<ChatDto>?> GetUserChatsFromCacheAsync(string userId, CancellationToken ct);
    Task SetUserChatsToCacheAsync(string userId, List<ChatDto> chats, TimeSpan expiration, CancellationToken ct);
    Task RemoveUserChatFromCacheAsync(string chatId, CancellationToken ct);
    Task<List<MessageDto>?> GetChatMessagesFromCacheAsync(string chatId, CancellationToken ct);
    Task AddMessageToCacheAsync(string chatId, MessageDto message, TimeSpan expiration, CancellationToken ct);
    Task AddMessagesToCacheAsync(string chatId, List<MessageDto> messages, TimeSpan expiration, CancellationToken ct);
    Task RemoveChatMessageFromCacheAsync(string chatId, MessageDto message, CancellationToken ct);

}