namespace GenAIPlatform.Infrastructure.Mcp;

/// <summary>
/// Lifecycle status of an external MCP server connection. Replaces a plain availability
/// boolean so a later connection policy (circuit breaker / backoff) can express richer
/// states without changing the snapshot contract. Today only <see cref="Available"/> and
/// <see cref="Unavailable"/> are produced; <see cref="Connecting"/> and <see cref="Paused"/>
/// are reserved for that policy.
/// </summary>
internal enum ExternalMcpServerStatus
{
    Available,
    Unavailable,
    Connecting,
    Paused
}
