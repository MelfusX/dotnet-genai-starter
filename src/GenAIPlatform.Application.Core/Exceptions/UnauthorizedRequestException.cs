namespace GenAIPlatform.Application.Core.Exceptions;

public sealed class UnauthorizedRequestException : AppException
{
    public UnauthorizedRequestException(string message)
        : base(message)
    {
    }

    public UnauthorizedRequestException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
