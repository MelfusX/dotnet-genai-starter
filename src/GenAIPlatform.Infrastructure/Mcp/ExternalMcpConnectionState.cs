namespace GenAIPlatform.Infrastructure.Mcp;

internal sealed class ExternalMcpConnectionState
{
    private readonly object gate = new();
    private readonly Dictionary<string, ExternalMcpServerConnection> connections = new(StringComparer.Ordinal);
    private IReadOnlyList<ExternalMcpServerSnapshot> snapshots = [];

    public IReadOnlyList<ExternalMcpServerSnapshot> GetSnapshots()
    {
        lock (gate)
        {
            return snapshots;
        }
    }

    public void ReplaceSnapshots(IReadOnlyList<ExternalMcpServerSnapshot> nextSnapshots)
    {
        lock (gate)
        {
            snapshots = nextSnapshots;
        }
    }

    public void SetConnection(string serverName, ExternalMcpServerConnection connection)
    {
        lock (gate)
        {
            connections[serverName] = connection;
        }
    }

    public bool TryGetConnection(string serverName, out ExternalMcpServerConnection? connection)
    {
        lock (gate)
        {
            return connections.TryGetValue(serverName, out connection);
        }
    }

    public ExternalMcpServerConnection? RemoveConnection(string serverName)
    {
        lock (gate)
        {
            if (!connections.Remove(serverName, out var connection))
            {
                return null;
            }

            return connection;
        }
    }

    public void MarkAvailability(string serverName, bool isAvailable)
    {
        lock (gate)
        {
            snapshots = snapshots
                .Select(snapshot => string.Equals(snapshot.ServerName, serverName, StringComparison.Ordinal)
                    ? snapshot with { IsAvailable = isAvailable }
                    : snapshot)
                .ToArray();
        }
    }

    public IReadOnlyList<ExternalMcpServerConnection> ClearConnections()
    {
        lock (gate)
        {
            var current = connections.Values.ToArray();
            connections.Clear();
            snapshots = snapshots.Select(static snapshot => snapshot with { IsAvailable = false }).ToArray();
            return current;
        }
    }
}