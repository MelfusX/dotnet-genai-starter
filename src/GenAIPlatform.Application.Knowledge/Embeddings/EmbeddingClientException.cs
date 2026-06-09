using System.Net;
using GenAIPlatform.Application.Core.Errors;

namespace GenAIPlatform.Application.Knowledge.Embeddings;

public sealed class EmbeddingClientException : ProviderException
{
    public EmbeddingClientException(
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
