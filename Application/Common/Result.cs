using Domain.Enums;

namespace Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; }
    public string? ErrorMessage { get; } 
    public ErrorType? ErrorType { get; }

    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
        ErrorMessage = null;
        ErrorType = null;
    }

    private Result(string errorMessage, ErrorType errorType)
    {
        IsSuccess = false;
        ErrorMessage = errorMessage;
        ErrorType = errorType;
        Value = default;
    }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(string errorMessage, ErrorType errorType) => new(errorMessage, errorType);
}

