namespace GenAIPlatform.Application.Core.Exceptions;

public abstract class ValidationException : AppException
{
    protected ValidationException(string message)
        : base(message)
    {
    }

    protected ValidationException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
