namespace GenAIPlatform.Api;

internal static class MultipartFormErrors
{
    public static bool IsMultipartBodyLimitExceeded(InvalidDataException exception)
    {
        return exception.Message.Contains(
            "Multipart body length limit",
            StringComparison.OrdinalIgnoreCase);
    }
}
