namespace GenAIPlatform.Application.Knowledge.Documents;

public sealed class DocumentStorageLimitExceededException(long maxSizeBytes)
    : Exception($"Document file must be {maxSizeBytes} bytes or fewer.")
{
    public long MaxSizeBytes { get; } = maxSizeBytes;
}
