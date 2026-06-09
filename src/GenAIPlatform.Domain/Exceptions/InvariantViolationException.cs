namespace GenAIPlatform.Domain.Exceptions;

public sealed class InvariantViolationException : DomainException
{
    public InvariantViolationException(string message)
        : base(message)
    {
    }

    public InvariantViolationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
