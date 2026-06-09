namespace GenAIPlatform.Application.Core.Exceptions;

public sealed class ForbiddenRequestException : AppException
{
    public ForbiddenRequestException(string message)
        : base(message)
    {
    }

    public ForbiddenRequestException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
