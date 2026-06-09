using GenAIPlatform.Application.Core.Exceptions;

namespace GenAIPlatform.Application.Knowledge.Documents;

public sealed class DocumentValidationException : ValidationException
{
    public DocumentValidationException(string message)
        : base(message)
    {
    }

    public DocumentValidationException(string message, Exception? innerException)
        : base(message, innerException)
    {
    }
}
