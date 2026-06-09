using System.Net;

namespace GenAIPlatform.Application.Core.Errors;

public abstract class ProviderException : GenAIPlatform.Application.Core.Exceptions.AppException
{
    protected ProviderException(
        string provider,
        string message,
        string? errorCode = null,
        HttpStatusCode? statusCode = null,
        string? providerErrorCode = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Provider = provider;
        ErrorCode = errorCode;
        StatusCode = statusCode;
        ProviderErrorCode = providerErrorCode;
    }

    public string Provider { get; }

    public string? ErrorCode { get; }

    public HttpStatusCode? StatusCode { get; }

    public string? ProviderErrorCode { get; }
}
