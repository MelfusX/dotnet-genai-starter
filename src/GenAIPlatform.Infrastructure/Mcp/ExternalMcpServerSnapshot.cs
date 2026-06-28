namespace GenAIPlatform.Infrastructure.Mcp;

internal sealed record ExternalMcpServerSnapshot(
    string ServerName,
    int Order,
    bool IsAvailable,
    IReadOnlyList<ExternalMcpToolSnapshot> Tools);