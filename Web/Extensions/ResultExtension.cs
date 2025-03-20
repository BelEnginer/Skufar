using Application.Common;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Web.Extensions;

public static class ResultExtension
{
    public static IActionResult HandleError<T>(this ControllerBase controller, Result<T> result)
    {
        if (result.IsSuccess)
            return null!; 

        return result.ErrorType switch
        {
            ErrorType.NotFound => controller.NotFound(result.ErrorMessage),
            ErrorType.Unauthorized => controller.Unauthorized(result.ErrorMessage),
            ErrorType.Conflict => controller.Conflict(result.ErrorMessage),
            _ => controller.StatusCode(500, "Unexpected error")
        };
    }
}