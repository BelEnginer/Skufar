using Application.Abstractions.IHubs;
using Application.Abstractions.IServices;
using Application.Abstractions.IUnitOfWork;
using Application.Common;
using Application.DTOs.GetDtos;
using Application.DTOs.PostDtos;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class ChatService(IUnitOfWork _repository,
    IMapper _mapper, 
    IChatCacheService _chatCache,
    ILogger<ChatService> _logger/*,
    IChatHub _chatHub*/) : IChatService
{
    public async Task<Result<List<ChatDto>>> GetUserChatsAsync(Guid userId, CancellationToken ct)
    {
        var cachedChats = await _chatCache.GetUserChatsFromCacheAsync(userId.ToString(), ct);
        if (cachedChats is not null)
        {
            _logger.LogInformation("User {UserId} has chats in cache", userId);
            return Result<List<ChatDto>>.Success(cachedChats);
        }
        _logger.LogInformation("User {UserId} has no chats in cache, loading from database...", userId);
        var chatsFromDb = await _repository.ChatRepository.GetChatsByUserIdAsync(userId, ct);
        if (chatsFromDb.Count == 0)
        {
            _logger.LogWarning("User {UserId} has no chats in database", userId);
            return Result<List<ChatDto>>.Failure("not found chats in database",ErrorType.NotFound);
        }
        var chats = _mapper.Map<List<ChatDto>>(chatsFromDb);
        await _chatCache.SetUserChatsToCacheAsync(userId.ToString(), chats, TimeSpan.FromMinutes(45), ct);
        return Result<List<ChatDto>>.Success(chats);
    }

    public async Task<Result<List<MessageDto>>> GetMessagesByChatIdAsync(Guid chatId, CancellationToken ct/*, int pageSize = 20, int page = 1*/)
    {
        var cachedMessages = await _chatCache.GetChatMessagesFromCacheAsync(chatId.ToString(), ct);
        if (cachedMessages is not null)
        {
            _logger.LogInformation("Chat {ChatId} has chat messages in cache", chatId);
            return Result<List<MessageDto>>.Success(cachedMessages);
        }
        _logger.LogInformation("Chat {ChatId} has no messages in cache, loading from database...", chatId);
        var messagesFromDb = await _repository.ChatRepository.GetMessagesByChatIdAsync(chatId, ct);
        if (messagesFromDb.Count == 0)
        {
            _logger.LogWarning("Chat {ChatId} has no messages in database", chatId);
            return Result<List<MessageDto>>.Failure("not found messages in database",ErrorType.NotFound);
        }
        var messages = _mapper.Map<List<MessageDto>>(messagesFromDb);
        await _chatCache.AddMessagesToCacheAsync(chatId.ToString(), messages, TimeSpan.FromMinutes(45), ct);
        return Result<List<MessageDto>>.Success(messages);
    }

    public async Task<Result<MessageDto>> SendMessageAsync(MessagePostDto messagePostDto, CancellationToken ct)
    {
        Chat? chat = null;
        if (messagePostDto.ChatId.HasValue)
        {
            chat = await _repository.ChatRepository.GetChatByIdAsync(messagePostDto.ChatId.Value, ct);
        }
        chat ??= await _repository.ChatRepository.GetChatByUserIdsAsync(
            messagePostDto.SenderId, messagePostDto.ReceiverId, ct);
        if (chat is null)
        {
            chat = new Chat
            {
                User1Id = messagePostDto.SenderId,
                User2Id = messagePostDto.ReceiverId
            };
            await _repository.ChatRepository.CreateChatAsync(chat, ct);
            await _repository.SaveAsync();
            _logger.LogInformation("Created new chat {ChatId} between {SenderId} and {ReceiverId}", 
                chat.Id, messagePostDto.SenderId, messagePostDto.ReceiverId);
        }
        var receiverId = chat.User1Id == messagePostDto.SenderId 
            ? chat.User2Id 
            : chat.User1Id;
    
        if (receiverId == Guid.Empty)
        {
            _logger.LogError("ReceiverId is null for chat {ChatId}", chat.Id);
            return Result<MessageDto>.Failure("Invalid receiver ID", ErrorType.Conflict);
        }
        var message = await _repository.ChatRepository.SendMessageAsync(chat.Id,
            messagePostDto.SenderId, 
            messagePostDto.Content, ct);
        _logger.LogInformation("Message {MessageId} sent to database", message.Id);
        var messageDto = _mapper.Map<MessageDto>(message);
        await _chatCache.AddMessageToCacheAsync(chat.Id.ToString(), messageDto, TimeSpan.FromMinutes(45), ct);
        _logger.LogInformation("Message {MessageId} sent to cache", message.Id);
        //await _chatHub.SendMessage(receiverId.ToString(), messageDto, ct);
        return Result<MessageDto>.Success(messageDto);
    }

    public async Task<Result<Unit>> DeleteChatAsync(Guid chatId, CancellationToken ct)
    {
        var chat = await _repository.ChatRepository.GetChatByIdAsync(chatId, ct);
        if (chat is null)
        {
            _logger.LogError("Chat {ChatId} was not found", chatId);
            return Result<Unit>.Failure("Chat not found", ErrorType.NotFound);
        }
        await _repository.ChatRepository.DeleteChatAsync(chat, ct);
        _logger.LogInformation("Chat {ChatId} was deleted in database", chatId);
        await _chatCache.RemoveUserChatFromCacheAsync(chatId.ToString(), ct);
        _logger.LogInformation("Chat {ChatId} was deleted from cache", chatId);
        return Result<Unit>.Success(Unit.Value);
    }

    public async Task<Result<Unit>> DeleteMessageAsync(Guid chatId,Guid messageId, CancellationToken ct)
    {
        var message = await _repository.ChatRepository.GetMessageByIdAsync(messageId, ct);
        var chat = await _repository.ChatRepository.GetChatByIdAsync(chatId, ct);
        if (message is null)
        {
            _logger.LogWarning("Message {MessageId} was not found", messageId);
            return Result<Unit>.Failure("Message not found", ErrorType.NotFound);
        }  
        if (chat is null)
        {
            _logger.LogWarning("Chat {ChatId} was not found", chatId);
            return Result<Unit>.Failure("Chat not found", ErrorType.NotFound);
        }
        var receiverId = message.SenderId == chat?.User1Id 
            ? chat.User2Id 
            : chat.User1Id;
        var messageDto = _mapper.Map<MessageDto>(message);
        await _repository.ChatRepository.DeleteMessageAsync(message, ct);
        _logger.LogInformation("Message {MessageId} was deleted", messageId);
        await _chatCache.RemoveChatMessageFromCacheAsync(chatId.ToString(), messageDto, ct);
        _logger.LogInformation("Message {MessageId} was deleted from cache", messageId);
        //await _chatHub.DeleteMessage(receiverId.ToString(), messageDto, ct);
        return Result<Unit>.Success(Unit.Value);
    }
}