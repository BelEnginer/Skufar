using Application.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
namespace Web.Middleware;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> _logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            InfrastructureException => StatusCodes.Status500InternalServerError,
            UnexpectedException => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };
        var title = exception switch
        {
            InfrastructureException => "Internal Server Error",
            UnexpectedException => "Unexpected Error",
            _ => "Unhandled Error"
        };
        _logger.LogError(exception, 
            "Exception: {Message}, Path: {Path}, Method: {Method}, Headers: {Headers}", 
            exception.Message, 
            httpContext.Request.Path,
            httpContext.Request.Method,
            httpContext.Request.Headers);
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = exception.Message
        };
        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}