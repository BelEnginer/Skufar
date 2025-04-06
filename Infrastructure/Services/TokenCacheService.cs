using Application.Abstractions.IServices;
using Application.Exceptions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class TokenCacheService(IDistributedCache _cache,ILogger<TokenCacheService> _logger) : ITokenCacheService
{
    public async Task SetRefreshTokenAsync(string refreshToken,string userId, TimeSpan expiry, CancellationToken ct)
    {
        try
        {
            var key = $"RefreshToken:{refreshToken}"; 
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry
            };
            _logger.LogInformation("Setting refresh token for user {UserId} with expiry {Expiry}", userId, expiry);
            await _cache.SetStringAsync(key, userId, options,ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to set refresh token for user {UserId}", userId);
            throw new InfrastructureException("Failed to set refresh token", ex);
        }
    }
    public async Task<string?> GetUserIdByRefreshTokenAsync(string refreshToken, CancellationToken ct)
    {
        try
        {
            var userIdString = await _cache.GetStringAsync(refreshToken, ct);
            if (string.IsNullOrEmpty(userIdString))
            {
                _logger.LogWarning("No user ID found for provided refresh token");
                return null;
            }
            _logger.LogInformation("Retrieved user ID {UserId} using refresh token", userIdString);
            return userIdString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user ID using refresh token");
            throw new InfrastructureException("Failed to get user ID", ex);
        }
    }

    public async Task RemoveRefreshTokenAsync(string userId, CancellationToken ct)
    {
        try
        {
            _logger.LogInformation("Removing refresh token from cache");
            await _cache.RemoveAsync(userId,ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove refresh token");
            throw new InfrastructureException("Failed to remove refresh token", ex);
        }
    }
}
