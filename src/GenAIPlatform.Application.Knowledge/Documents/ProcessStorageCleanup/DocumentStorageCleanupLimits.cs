namespace GenAIPlatform.Application.Knowledge.Documents;

internal static class DocumentStorageCleanupLimits
{
    public static int ResolveMaxRequests(int? requestedMaxRequests, int configuredMaxRequests)
    {
        var maxRequests = requestedMaxRequests ?? configuredMaxRequests;
        return Math.Clamp(maxRequests, 1, 50);
    }
}
