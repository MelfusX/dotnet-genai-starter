namespace GenAIPlatform.Application.Core.Exceptions;

public sealed class ConflictException : AppException
{
    public ConflictException(string message)
        : base(message)
    {
    }

    public ConflictException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
