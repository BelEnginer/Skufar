using Application.Common;
using Application.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Infrastructure.Helpers;

public static class ErrorHandlingHelper
{
    public static async Task<Result<T>> ExecuteAsync<T>(Func<Task<Result<T>>> action,Microsoft.Extensions.Logging.ILogger _logger)
    {
        try
        {
            return await action();
        }
        catch (OperationCanceledException e)
        {
            _logger.LogWarning(e, "Operation was cancelled");
            throw new InfrastructureException("Operation cancelled", e);
        }
        catch (DbUpdateException e)
        {
            _logger.LogError(e, "Database exception occurred");
            throw new InfrastructureException("Database exception", e);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected exception occurred");
            throw new UnexpectedException("Unexpected exception", e);
        }
        
    }
}