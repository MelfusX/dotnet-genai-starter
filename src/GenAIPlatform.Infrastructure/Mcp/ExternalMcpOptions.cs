namespace GenAIPlatform.Infrastructure.Mcp;

public sealed class ExternalMcpOptions
{
    public const string SectionName = "GenAIPlatform:ExternalMcp";

    public List<ExternalMcpServerOptions> Servers { get; init; } = [];
}