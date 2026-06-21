namespace GenAIPlatform.Application.Agentic.Tools.Execution;

internal static class AgentToolSchemaVersion
{
    public static string Resolve(string? schemaVersion)
    {
        return string.IsNullOrWhiteSpace(schemaVersion) ? "v1" : schemaVersion;
    }
}
