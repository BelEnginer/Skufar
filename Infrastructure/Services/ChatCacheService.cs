using System.Text.Json;
using Application.Abstractions.IServices;
using Application.DTOs.GetDtos;
using Application.Exceptions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class ChatCacheService(IDistributedCache _cache,ILogger<ChatCacheService> _logger) : IChatCacheService
{
       public async Task<List<ChatDto>?> GetUserChatsFromCacheAsync(string userId, CancellationToken ct)
       {
        var key = $"UserChats:{userId}";
        try
        {
            var chatsJson = await _cache.GetStringAsync(key, ct);
            if (string.IsNullOrEmpty(chatsJson))
            {
                _logger.LogInformation("Cache miss for user {UserId}", userId);
                return null;
            }

            List<ChatDto>? chats;
            try
            {
                chats = JsonSerializer.Deserialize<List<ChatDto>>(chatsJson);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize user chats for {UserId}. Resetting cache.", userId);
                return null;
            }

            if (chats is null || chats.Count == 0)
            {
                _logger.LogWarning("Deserialized list is empty for user {UserId}", userId);
                return null;
            }

            _logger.LogInformation("Found {ChatCount} chats for user {UserId}", chats.Count, userId);
            return chats;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving user chats from cache for user {UserId}", userId);
            throw new InfrastructureException("An error occurred while retrieving user chats from cache", ex);
        }
       }

       public async Task SetUserChatToCacheAsync(string userId, ChatDto chat, TimeSpan expiration, CancellationToken ct)
       {
           var key = $"UserChats:{userId}";
           try
           {
               List<ChatDto> chats = [];
               var existingChatsJson = await _cache.GetStringAsync(key, ct);
               if (!string.IsNullOrEmpty(existingChatsJson))
               {
                   try
                   {
                       chats = JsonSerializer.Deserialize<List<ChatDto>>(existingChatsJson) ?? [];
                   }
                   catch (JsonException jsonEx)
                   {
                       _logger.LogError(jsonEx, "Failed to deserialize chats for user {UserId}. Resetting cache.", userId);
                       chats = [];
                   }
               }
               chats.Add(chat);
               var options = new DistributedCacheEntryOptions()
               {
                   AbsoluteExpirationRelativeToNow = expiration
               };
               var serializedChats = JsonSerializer.Serialize(chats);
               await _cache.SetStringAsync(key, serializedChats, options, ct);
               _logger.LogInformation("Added new chat to cache for user {UserId}. Total chats: {ChatCount}", userId, chats.Count);
           }
           catch (Exception ex)
           {
               _logger.LogError(ex, "Error while saving chat to cache for user {UserId}", userId);
               throw new InfrastructureException("Error while saving chat to cache", ex);
           }
       }

       public async Task SetUserChatsToCacheAsync(string userId, List<ChatDto> chats, TimeSpan expiration, CancellationToken ct)
       {
           var key = $"UserChats:{userId}";
           try
           {
               var options = new DistributedCacheEntryOptions()
               {
                   AbsoluteExpirationRelativeToNow = expiration
               };
               var serializedChats  = JsonSerializer.Serialize(chats);
               await _cache.SetStringAsync(key, serializedChats, options, ct);
               _logger.LogInformation("Added {ChatCount} chats to cache for user {UserId}", chats.Count, userId);
           }
           catch (Exception ex)
           {
               _logger.LogError(ex,"Error while saving chats to cache for user {userId}", userId);
               throw new InfrastructureException("Error while saving chats to cache", ex);
           }
       }

       public async Task RemoveUserChatFromCacheAsync(string chatId, CancellationToken ct)
       {
           var key = $"UserChat:{chatId}";
           try
           {
               if (await _cache.GetStringAsync(key, ct) is null)
               {
                   _logger.LogWarning("No chats found in cache for user {UserId}", chatId);
                   return;
               }

               await _cache.RemoveAsync(key, ct);
               _logger.LogInformation("Removed all chats from cache for user {UserId}", chatId);
           }
           catch (Exception ex)
           {
               _logger.LogError(ex, "Error while removing chats for user {UserId} from cache", chatId);
               throw new InfrastructureException("Error while removing user chats from cache", ex);
           }
       }


       public async Task<List<MessageDto>?> GetChatMessagesFromCacheAsync(string chatId, CancellationToken ct)
       {
           var key = $"ChatMessages:{chatId}";
           try
           {
               var messagesJson = await _cache.GetStringAsync(key, ct);
               if (string.IsNullOrEmpty(messagesJson))
               {
                   _logger.LogInformation("Cache miss for chat {ChatId}", chatId);
                   return null;
               }
               List<MessageDto>? deserializedMessages;
               try
               {
                   deserializedMessages = JsonSerializer.Deserialize<List<MessageDto>>(messagesJson);
               }
               catch (JsonException jsonEx)
               {
                   _logger.LogError(jsonEx, "Failed to deserialize messages for chat {ChatId}", chatId);
                   return null;
               }
               if (deserializedMessages is null || deserializedMessages.Count == 0)
               {
                   _logger.LogWarning("Deserialized list is empty for chat {ChatId}", chatId);
                   return null;
               }
               _logger.LogInformation("Found {MessageCount} messages for chat {ChatId}", deserializedMessages.Count, chatId);
               return deserializedMessages;
           }
           catch (Exception ex)
           {
               _logger.LogError(ex, "Error while retrieving messages from chat {ChatId}", chatId);
               throw new InfrastructureException("Error while retrieving messages from chat", ex);
           }
       }
    
       public async Task AddMessageToCacheAsync(string chatId, MessageDto message, TimeSpan expiration, CancellationToken ct)
       {
           try
           {
               _logger.LogInformation("Attempting to add message {MessageId} to cache under chat {ChatId}", message.Id, chatId);
               var existingMessages = await _cache.GetStringAsync($"AppCache:ChatMessages:{chatId}", ct);
               var messages = existingMessages != null
                   ? JsonSerializer.Deserialize<List<MessageDto>>(existingMessages) ?? []
                   : [];
               messages.Add(message);
               var serializedMessages = JsonSerializer.Serialize(messages);
               await _cache.SetStringAsync($"ChatMessages:{chatId}", serializedMessages, new DistributedCacheEntryOptions
               {
                   AbsoluteExpirationRelativeToNow = expiration
               }, ct);

               _logger.LogInformation("Successfully added message {MessageId} to cache for chat {ChatId}", message.Id, chatId);
           }
           catch (Exception ex)
           {
               _logger.LogError(ex, "Failed to add message {MessageId} to cache for chat {ChatId}", message.Id, chatId);
           }
       }


       public async Task AddMessagesToCacheAsync(string chatId, List<MessageDto> messages, TimeSpan expiration, CancellationToken ct)
       {
           var key = $"ChatMessages:{chatId}";
           try
           {
               var options = new DistributedCacheEntryOptions()
               {
                   AbsoluteExpirationRelativeToNow = expiration
               };
               var serializedMessages  = JsonSerializer.Serialize(messages);
               await _cache.SetStringAsync(key, serializedMessages, options, ct);
               _logger.LogInformation("Added {MessagesCount} messages to cache in chat {ChatId}", messages.Count, chatId);
           }
           catch (Exception ex)
           {
               _logger.LogError(ex,"Error while saving messages to cache in cache {ChatId}", chatId);
               throw new InfrastructureException("Error while saving  messages to cache", ex);
           }
       }

       public async Task RemoveChatMessageFromCacheAsync(string chatId, MessageDto message, CancellationToken ct)
        {
            var key = $"ChatMessages:{chatId}";
            try
            {
                var existingMessagesJson = await _cache.GetStringAsync(key, ct);
                if (string.IsNullOrEmpty(existingMessagesJson))
                {
                    _logger.LogWarning("No messages found in cache for chat {ChatId}", chatId);
                    return;
                }

                List<MessageDto> messages;
                try
                {
                    messages = JsonSerializer.Deserialize<List<MessageDto>>(existingMessagesJson) ?? [];
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "Failed to deserialize messages for chat {ChatId}. Resetting cache.", chatId);
                    return;
                }
                var messageCountBefore = messages.Count;
                messages.RemoveAll(m => m.Id == message.Id);
                if (messages.Count == messageCountBefore)
                {
                    _logger.LogWarning("Message {MessageId} not found in cache for chat {ChatId}", message.Id, chatId);
                    return;
                }
                if (messages.Count == 0)
                {
                    await _cache.RemoveAsync(key, ct);
                    _logger.LogInformation("All messages removed from cache for chat {ChatId}", chatId);
                    return;
                }
                var serializedMessages = JsonSerializer.Serialize(messages);
                await _cache.SetStringAsync(key, serializedMessages, ct);
                _logger.LogInformation("Removed message {MessageId} from cache for chat {ChatId}", message.Id, chatId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while removing message {MessageId} from chat {ChatId}", message.Id, chatId);
                throw new InfrastructureException("Error while removing message from chat", ex);
            }
        }
}