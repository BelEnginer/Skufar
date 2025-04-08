using System.Security.Claims;

namespace Web.Extensions;

public static class HttpContextExtension
{
    public static Guid GetUserId(this HttpContext httpContext)
    {
    var userIdClaim = httpContext.User.FindFirst("userId") ?? 
                      httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
    if (userIdClaim is null)
        throw new UnauthorizedAccessException("UserId claim is missing");
    
    return Guid.Parse(userIdClaim.Value);
    }
}