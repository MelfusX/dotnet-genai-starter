using GenAIPlatform.Application.Core.Exceptions;

namespace GenAIPlatform.Application.Knowledge.Documents;

public sealed class DocumentTooLargeException : ValidationException
{
    public DocumentTooLargeException(string message)
        : base(message)
    {
    }

    public DocumentTooLargeException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
