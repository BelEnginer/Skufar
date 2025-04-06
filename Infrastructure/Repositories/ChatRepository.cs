using System.Linq.Expressions;
using Application.Abstractions.IRepositories;
using Domain.Models;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ChatRepository(ApplicationDbContext context) : BaseRepository<Chat>(context), IChatRepository
{
    public async Task<Chat?> GetChatByUserIdsAsync(Guid userId1, Guid userId2, CancellationToken ct) =>
        await context.Chats
            .AsNoTracking()
            .FirstOrDefaultAsync(c => (c.User1Id == userId1 && c.User2Id == userId2) ||
                                      (c.User1Id == userId2 && c.User2Id == userId1), ct);

    public async Task<Chat?> GetChatByIdAsync(Guid chatId, CancellationToken ct) =>
        await context.Chats
            .AsNoTracking()
            .FirstOrDefaultAsync(id => id.Id == chatId, ct);



    public async Task CreateChatAsync(Chat chat, CancellationToken ct)
    {
        chat.Id = Guid.NewGuid();
        await context.Chats.AddAsync(chat, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<List<Message>> GetMessagesByChatIdAsync(Guid chatId, CancellationToken ct) =>
        await context.Messages
            .Where(m => m.ChatId == chatId)
            .OrderBy(m => m.Date)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<Message?> SendMessageAsync(Guid chatId, Guid senderId, string content, CancellationToken ct)
    {
        var newMessage = new Message
        {
            Id = Guid.NewGuid(),
            ChatId = chatId,
            SenderId = senderId,
            Content = content,
            Date = DateTime.UtcNow,
            IsRead = false
        };
        await context.Messages.AddAsync(newMessage, ct);
        var chat = await context.Chats.FindAsync([chatId], ct);
        if (chat is not null) 
        {
            chat.LastActivity = DateTime.UtcNow;
            context.Chats.Update(chat);
        }
        await context.SaveChangesAsync(ct);
        return newMessage;
    }

    public async Task<List<Chat>> GetChatsByUserIdAsync(Guid userId, CancellationToken ct) =>
        await context.Chats
            .Where(c => c.User1Id == userId || c.User2Id == userId)
            .Include(c => c.Messages)
            .OrderByDescending(c => c.LastActivity)
            .ToListAsync(ct);
 
    
    public async Task<Message?> GetMessageByIdAsync(Guid messageId, CancellationToken ct) =>
        await context.Messages
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == messageId, ct);

    public async Task DeleteChatAsync(Chat chat, CancellationToken ct)
    {
        Delete(chat);
        await context.SaveChangesAsync(ct);
    }
    public async Task DeleteMessageAsync(Message message, CancellationToken ct)
    {
        context.Messages.Remove(message);
        await context.SaveChangesAsync(ct);
    }
}
