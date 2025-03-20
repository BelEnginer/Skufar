    using Application.DTOs;
using Application.DTOs.PostDtos;
using Application.DTOs.UpdateDtos;
using Application.IServices;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Web.Extensions;

namespace Web.Controllers;
[ApiController]
[Route("api/[controller]")]
public class UserController(IUserService userService) : ControllerBase
{
    [HttpPost("Register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegisterDto userRegisterDto,CancellationToken ct)
    {
        var result = await userService.RegisterAsync(userRegisterDto,ct);
        var errorResponse = this.HandleError(result);
        return errorResponse != null ? errorResponse : Created("users/register", result.Value);
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken ct)
    {
        var result = await userService.LoginAsync(loginDto,ct);
        var errorResponse = this.HandleError(result);
        Response.Cookies.Append("AccessToken", result.Value.AccessToken, new CookieOptions
        {
            HttpOnly = true, 
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddHours(1)
        });
        return errorResponse != null ? errorResponse : Ok(new { message = "success", refreshToken = result.Value.RefreshToken });
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh([FromBody] AuthResponseDto authResponseDto, CancellationToken ct)
    {
        var result = await userService.RefreshTokenAsync(authResponseDto,ct);
        var errorResponse = this.HandleError(result);
        return errorResponse != null ? errorResponse : Ok(new { message = "success refreshing" });
    }

    //[Authorize]
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUserById(Guid userId, CancellationToken ct)
    {
        var result = await userService.GetUserByIdAsync(userId,ct);
        var errorResponse = this.HandleError(result);
        return errorResponse != null ? errorResponse : Ok(result.Value);
    }

    //[Authorize]
    [HttpPatch("{userId:guid}")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UserUpdateDto userUpdateDto, CancellationToken ct)
    {
        var result = await userService.UpdateAsync(userId, userUpdateDto,ct);
        var errorResponse = this.HandleError(result);
        return errorResponse != null ? errorResponse : Ok(new { message = "User updated", userId });
    }
}
