namespace Application.Exceptions;

public class InfrastructureException(string message,Exception innerException) : Exception(message,innerException);