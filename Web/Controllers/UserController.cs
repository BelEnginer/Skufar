using Application.Abstractions.IServices;
using Application.DTOs;
using Application.DTOs.PostDtos;
using Application.DTOs.UpdateDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;
[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService _userService) : ControllerBase
{
    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDto userRegisterDto,CancellationToken ct)
    {
        var result = await _userService.RegisterAsync(userRegisterDto,ct);
        return result.IsSuccess ? Created("users/register", result.Value) : BadRequest(result.ErrorMessage);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken ct)
    {
        var result = await _userService.LoginAsync(loginDto,ct);
        return result.IsSuccess ? Ok(new { message = "success", accessTken = result.Value.AccessToken }) : BadRequest(result.ErrorMessage);
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh([FromBody] AuthResponseDto authResponseDto, CancellationToken ct)
    {
        var result = await _userService.RefreshTokenAsync(authResponseDto,ct);
        return result.IsSuccess ? Ok(new { message = "success refreshing" }) : BadRequest(result.ErrorMessage);
    }

    [Authorize]
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUserById(Guid userId, CancellationToken ct)
    {
        var result = await _userService.GetUserByIdAsync(userId,ct);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ErrorMessage);
    }

    [Authorize]
    [HttpPatch("{userId:guid}")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserUpdateDto userUpdateDto, CancellationToken ct)
    {
        var result = await _userService.UpdateAsync(userId, userUpdateDto,ct);
        return result.IsSuccess ? Ok(new { message = "User updated", userId }) : BadRequest(result.ErrorMessage);
    }
}
