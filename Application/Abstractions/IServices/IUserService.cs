using Application.Common;
using Application.DTOs;
using Application.DTOs.GetDtos;
using Application.DTOs.PostDtos;
using Application.DTOs.UpdateDtos;

namespace Application.Abstractions.IServices;

public interface IUserService
{
     Task<Result<UserDto>> RegisterAsync(UserRegisterDto userRegisterDto,CancellationToken ct);
     Task<Result<AuthResponseDto>> LoginAsync(LoginDto loginDto, CancellationToken ct);
     Task<Result<AuthResponseDto>> RefreshTokenAsync(AuthResponseDto authResponseDto, CancellationToken ct);
     Task<Result<UserDto>> GetUserByIdAsync(Guid userId,CancellationToken ct);
     Task<Result<Unit>> UpdateAsync(Guid userToUpdate,UserUpdateDto userUpdateDto,CancellationToken ct);
}