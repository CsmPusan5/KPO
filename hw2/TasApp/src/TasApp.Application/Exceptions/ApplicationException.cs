namespace TasApp.Application.Exceptions;

public abstract class TasApplicationException : Exception
{
    protected TasApplicationException(string message) : base(message) { }
    protected TasApplicationException(string message, Exception inner) : base(message, inner) { }
}