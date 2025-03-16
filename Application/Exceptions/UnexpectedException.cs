namespace Application.Exceptions;

public class UnexpectedException(string message,Exception innerException) : Exception(message,innerException);