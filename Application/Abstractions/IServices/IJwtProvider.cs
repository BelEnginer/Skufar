using Domain.Models;

namespace Application.Abstractions.IServices;

public interface IJwtProvider
{
     string GenerateAccessToken(User user);
     string GenerateRefreshToken();
}