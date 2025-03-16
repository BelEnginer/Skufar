using Application.Common;
using Domain.Entites;

namespace Application.IServices;

public interface IJwtProvider
{
     string GenerateAccessToken(User user);
     string GenerateRefreshToken();
}