using Application.Common;
using Domain.Models;

namespace Application.Abstractions.IRepositories;

public interface IChatRepository
{
    Task<Chat?> GetChatByUserIdsAsync(Guid userId1, Guid userId2, CancellationToken ct);
    Task<Chat?> GetChatByIdAsync(Guid chatId, CancellationToken ct);
    Task CreateChatAsync(Chat chat, CancellationToken ct);
    Task<List<Message>> GetMessagesByChatIdAsync(Guid chatId, CancellationToken ct);
    Task<Message?> SendMessageAsync(Guid chatId, Guid senderId, string content, CancellationToken ct);
    Task<Message?> GetMessageByIdAsync(Guid messageId, CancellationToken ct);
    Task DeleteChatAsync(Chat chat, CancellationToken ct);
    Task<List<Chat>> GetChatsByUserIdAsync(Guid userId, CancellationToken ct);
    Task DeleteMessageAsync(Message messages, CancellationToken ct);
}

