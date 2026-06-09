using System.Net;
using GenAIPlatform.Application.Core.Errors;

namespace GenAIPlatform.Application.Generation.ModelGateway;

public sealed class AiModelException : ProviderException
{
    public AiModelException(
        string provider,
        string message,
        string? errorCode = null,
        HttpStatusCode? statusCode = null,
        string? providerErrorCode = null,
        Exception? innerException = null)
        : base(provider, message, errorCode, statusCode, providerErrorCode, innerException)
    {
    }
}
