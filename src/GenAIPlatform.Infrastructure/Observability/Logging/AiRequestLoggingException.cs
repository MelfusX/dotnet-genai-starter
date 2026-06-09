namespace GenAIPlatform.Infrastructure.Observability.Logging;

public sealed class AiRequestLoggingException : Exception
{
    public AiRequestLoggingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
