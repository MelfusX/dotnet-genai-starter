namespace GenAIPlatform.Infrastructure.Mcp;

/// <summary>
/// Default connection policy: always permit connect attempts and ignore outcomes. The
/// resilient circuit-breaker / backoff policy is deferred (see the IncidentCompass IC-BL-015
/// plan) and can replace this registration without any other change.
/// </summary>
internal sealed class AlwaysConnectMcpPolicy : IExternalMcpConnectionPolicy
{
    public bool ShouldAttemptConnect(string serverName) => true;

    public void RecordConnectSuccess(string serverName)
    {
    }

    public void RecordConnectFailure(string serverName)
    {
    }
}
