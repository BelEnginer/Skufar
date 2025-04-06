using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Abstractions.IServices;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services;

public class JwtProvider(IConfiguration _configuration,ILogger<JwtProvider> _logger) : IJwtProvider
{
    private readonly string _jwtKey = _configuration["JWT:Key"] ?? throw new ArgumentNullException("JWT key is missing");

    public string GenerateAccessToken(User user)
    {
        if (_jwtKey.Length < 32)
        {
            _logger.LogError("JWT key is too short");
            throw new InvalidOperationException("JWT key must be at least 32 characters long for security reasons");
        }
        Claim[] claims = [new("userId", user.Id.ToString())];
        var secKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
        var signingCredentials = new SigningCredentials(secKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            expires: DateTime.UtcNow.AddHours(1)
        );
        _logger.LogInformation("Created access token to user {UserId}", user.Id);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }
        _logger.LogInformation("Generated a new refresh token");
        return Convert.ToBase64String(randomNumber)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
    }
}
