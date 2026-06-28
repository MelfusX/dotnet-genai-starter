namespace GenAIPlatform.Infrastructure.Mcp;

internal interface IExternalMcpConnectionManager
{
    IReadOnlyList<ExternalMcpServerSnapshot> GetSnapshots();

    Task<ExternalMcpToolCallResult> CallToolAsync(
        ExternalMcpToolSnapshot tool,
        IReadOnlyDictionary<string, object?>? arguments,
        CancellationToken cancellationToken);
}