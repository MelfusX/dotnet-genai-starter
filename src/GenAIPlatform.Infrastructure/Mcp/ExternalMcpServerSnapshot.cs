namespace GenAIPlatform.Infrastructure.Mcp;

internal sealed record ExternalMcpServerSnapshot(
    string ServerName,
    int Order,
    ExternalMcpServerStatus Status,
    IReadOnlyList<ExternalMcpToolSnapshot> Tools)
{
    public bool IsAvailable => Status == ExternalMcpServerStatus.Available;
}
