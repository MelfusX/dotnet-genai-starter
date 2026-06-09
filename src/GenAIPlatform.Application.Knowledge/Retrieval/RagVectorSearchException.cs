using GenAIPlatform.Application.Core.Errors;

namespace GenAIPlatform.Application.Knowledge.Retrieval;

public sealed class RagVectorSearchException : ProviderException
{
    public RagVectorSearchException(
        string provider,
        string message,
        string? errorCode = null,
        Exception? innerException = null)
        : base(provider, message, errorCode, statusCode: null, providerErrorCode: null, innerException)
    {
    }
}
