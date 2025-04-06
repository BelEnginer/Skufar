using Application.Abstractions.IServices;
using Application.Abstractions.IUnitOfWork;
using Application.Common;
using Application.DTOs;
using Application.DTOs.GetDtos;
using Application.DTOs.PostDtos;
using Application.DTOs.UpdateDtos;
using Application.Exceptions;
using AutoMapper;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Helpers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;
public class UserService(
    IUnitOfWork _repository,
    IMapper _mapper,
    IJwtProvider _jwtProvider,
    ITokenCacheService _tokenCache,
    ILogger<UserService> _logger)
    : IUserService
{
    public async Task<Result<UserDto>> RegisterAsync(UserRegisterDto userRegisterDto, CancellationToken ct)
    {
        return await ErrorHandlingHelper.ExecuteAsync(async () =>
        {
            var email = userRegisterDto.Email?.ToLower();
            var user = await _repository.UserRepository.GetUserByEmailAsync(email, ct);
            if (user != null)
            {
                _logger.LogWarning("User registration failed: Email already exists.");
                return Result<UserDto>.Failure("User with this email already exists", ErrorType.Conflict);
            }
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password);
            var userEntity = _mapper.Map<User>(userRegisterDto);
            userEntity.PasswordHash = passwordHash;
            userEntity.Email = email;
            await _repository.UserRepository.CreateUserAsync(userEntity, ct);
            _logger.LogInformation("User with id {UserId} registered.", userEntity.Id);
            return Result<UserDto>.Success(_mapper.Map<UserDto>(userEntity));
        }, _logger);
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto loginDto, CancellationToken ct)
    {
        return await ErrorHandlingHelper.ExecuteAsync(async () =>
        {
            var email = loginDto.Email.ToLower();
            var user = await _repository.UserRepository.GetUserByEmailAsync(email, ct);
            if (user == null)
            {
                _logger.LogWarning("Login failed: User not found.");
                return Result<AuthResponseDto>.Failure("Invalid Email", ErrorType.Unauthorized);
            }
            var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                _logger.LogWarning("Login failed: Invalid password.");
                return Result<AuthResponseDto>.Failure("Invalid Password", ErrorType.Unauthorized);
            }
            var accessToken = _jwtProvider.GenerateAccessToken(user);
            var refreshToken = _jwtProvider.GenerateRefreshToken();
            try
            {
                _logger.LogInformation("Setting refresh token for user {UserId}", user.Id);
                await _tokenCache.SetRefreshTokenAsync(refreshToken, user.Id.ToString(), TimeSpan.FromDays(7), ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to set refresh token for user {UserId}", user.Id);
                throw new InfrastructureException("Could not refresh token", ex);
            }
            var authResponseDto = new AuthResponseDto { AccessToken = accessToken, RefreshToken = refreshToken };
            _logger.LogInformation("User with id {UserId} logged in.", user.Id);
            return Result<AuthResponseDto>.Success(authResponseDto);
        }, _logger);
    }

    public async Task<Result<AuthResponseDto>> RefreshTokenAsync(AuthResponseDto authResponseDto, CancellationToken ct)
    {
        var userId = await _tokenCache.GetUserIdByRefreshTokenAsync(authResponseDto.RefreshToken, ct);
        if (userId == null)
        {
            _logger.LogWarning("Refresh token failed: User not found.");
            return Result<AuthResponseDto>.Failure("Invalid RefreshToken", ErrorType.Unauthorized);
        }
        var user = await _repository.UserRepository.GetUserByIdAsync(Guid.Parse(userId), ct);
        if (user == null)
        {
            _logger.LogWarning("Refresh token failed: User with id {UserId} not found.", userId);
            return Result<AuthResponseDto>.Failure("User unauthorized", ErrorType.Unauthorized);
        }
        var newAccessToken = _jwtProvider.GenerateAccessToken(user);
        var newRefreshToken = _jwtProvider.GenerateRefreshToken();
        await _tokenCache.RemoveRefreshTokenAsync(authResponseDto.RefreshToken, ct);
        await _tokenCache.SetRefreshTokenAsync(newRefreshToken, userId, TimeSpan.FromDays(7), ct);
        _logger.LogInformation("User with id {UserId} successfully refreshed token.", user.Id);
        return Result<AuthResponseDto>.Success(new AuthResponseDto { AccessToken = newAccessToken, RefreshToken = newRefreshToken });
    }

    public async Task<Result<UserDto>> GetUserByIdAsync(Guid userId, CancellationToken ct)
    {
        var user = await _repository.UserRepository.GetUserByIdAsync(userId, ct);
        if (user == null)
        {
            _logger.LogWarning("User with id {UserId} was not found.", userId);
            return Result<UserDto>.Failure("User not found", ErrorType.NotFound);
        }
        _logger.LogInformation("User with id {UserId} successfully found.", userId);
        return Result<UserDto>.Success(_mapper.Map<UserDto>(user));
    }

    public async Task<Result<Unit>> UpdateAsync(Guid userToUpdate, UserUpdateDto userUpdateDto, CancellationToken ct)
    {
        return await ErrorHandlingHelper.ExecuteAsync(async () =>
        {
            var user = await _repository.UserRepository.GetUserByIdAsync(userToUpdate, ct);
            if (user == null)
            {
                _logger.LogWarning("User with id {UserId} was not found.", userToUpdate);
                return Result<Unit>.Failure("User not found", ErrorType.NotFound);
            }
            _mapper.Map(userUpdateDto, user);
            await _repository.UserRepository.UpdateUserAsync(user, ct);
            _logger.LogInformation("User with id {UserId} successfully updated.", userToUpdate);
            return Result<Unit>.Success(Unit.Value);
        }, _logger);
    }
}

