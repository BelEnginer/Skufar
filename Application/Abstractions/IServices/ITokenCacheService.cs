
namespace Application.IServices;

public interface ITokenCacheService
{
         Task SetRefreshTokenAsync(string userId,string refreshToken,TimeSpan expiry,CancellationToken ct);
         Task<string?> GetUserIdByRefreshTokenAsync(string refreshToken,CancellationToken ct);
         Task RemoveRefreshTokenAsync(string refreshToken,CancellationToken ct);
}