using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GenAIPlatform.Infrastructure.Mcp;

/// <summary>
/// Establishes connections to external MCP servers: a bounded-parallel startup warmup and
/// on-demand reconnect, both gated by <see cref="IExternalMcpConnectionPolicy"/>. Connect order
/// is the configured server index, so parallel warmup never changes the deterministic tool
/// listing regardless of which server finishes connecting first.
/// </summary>
internal sealed class ExternalMcpConnector(
    IOptions<ExternalMcpOptions> options,
    IExternalMcpClientFactory clientFactory,
    IExternalMcpConnectionPolicy policy,
    ILogger logger)
{
    public async Task RefreshAsync(
        ExternalMcpConnectionState state,
        CancellationToken cancellationToken)
    {
        var servers = EnabledServers().ToArray();
        var current = state.GetSnapshots().ToDictionary(static snapshot => snapshot.ServerName, StringComparer.Ordinal);
        var maxParallel = Math.Max(1, options.Value.MaxParallelConnects);
        using var slots = new SemaphoreSlim(maxParallel, maxParallel);

        var attempts = new List<Task>(servers.Length);
        for (var index = 0; index < servers.Length; index++)
        {
            var server = servers[index];
            var serverName = ExternalMcpNameSanitizer.SanitizeSegment(server.Name, "server");

            // Leave a working server untouched: no reconnect, no re-list, stable snapshot hash.
            if (current.TryGetValue(serverName, out var existing) && existing.IsAvailable)
            {
                continue;
            }

            attempts.Add(ConnectAndUpsertAsync(slots, state, server, index, cancellationToken));
        }

        await Task.WhenAll(attempts);
    }

    private async Task ConnectAndUpsertAsync(
        SemaphoreSlim slots,
        ExternalMcpConnectionState state,
        ExternalMcpServerOptions server,
        int order,
        CancellationToken cancellationToken)
    {
        await slots.WaitAsync(cancellationToken);
        try
        {
            // Order is the configured index, so bounded-parallel attempts never change listing order.
            var snapshot = await ConnectAndSnapshotAsync(state, server, order, cancellationToken);
            state.UpsertSnapshot(snapshot);
        }
        finally
        {
            slots.Release();
        }
    }

    public async Task<ExternalMcpServerConnection?> GetOrReconnectAsync(
        ExternalMcpConnectionState state,
        string serverName,
        CancellationToken cancellationToken)
    {
        if (state.TryGetConnection(serverName, out var connection))
        {
            return connection;
        }

        var server = FindServer(serverName);
        if (server is null)
        {
            return null;
        }

        if (!policy.ShouldAttemptConnect(serverName))
        {
            state.MarkStatus(serverName, ExternalMcpServerStatus.Paused);
            return null;
        }

        try
        {
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(TimeSpan.FromSeconds(server.ConnectTimeoutSeconds));
            var client = await clientFactory.CreateAsync(server, timeout.Token);
            connection = new ExternalMcpServerConnection(server, client);
            state.SetConnection(serverName, connection);
            state.MarkStatus(serverName, ExternalMcpServerStatus.Available);
            policy.RecordConnectSuccess(serverName);
            return connection;
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            policy.RecordConnectFailure(serverName);
            logger.LogWarning(exception, "External MCP server {ServerName} reconnect failed.", server.Name);
            state.MarkStatus(serverName, ExternalMcpServerStatus.Unavailable);
            return null;
        }
    }

    private async Task<ExternalMcpServerSnapshot> ConnectAndSnapshotAsync(
        ExternalMcpConnectionState state,
        ExternalMcpServerOptions server,
        int order,
        CancellationToken cancellationToken)
    {
        var serverName = ExternalMcpNameSanitizer.SanitizeSegment(server.Name, "server");
        if (!policy.ShouldAttemptConnect(serverName))
        {
            return new ExternalMcpServerSnapshot(serverName, order, ExternalMcpServerStatus.Paused, []);
        }

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(server.ConnectTimeoutSeconds));

        IExternalMcpClient? client = null;
        try
        {
            client = await clientFactory.CreateAsync(server, timeout.Token);
            var descriptors = await client.ListToolsAsync(timeout.Token);
            var snapshot = ExternalMcpSnapshotBuilder.Build(server, order, descriptors, ExternalMcpServerStatus.Available);
            state.SetConnection(snapshot.ServerName, new ExternalMcpServerConnection(server, client));
            client = null;
            policy.RecordConnectSuccess(serverName);
            return snapshot;
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            if (client is not null)
            {
                await client.DisposeAsync();
            }

            policy.RecordConnectFailure(serverName);
            logger.LogWarning(exception, "External MCP server {ServerName} is unavailable.", server.Name);
            return new ExternalMcpServerSnapshot(serverName, order, ExternalMcpServerStatus.Unavailable, []);
        }
    }

    private ExternalMcpServerOptions? FindServer(string serverName)
    {
        return EnabledServers().FirstOrDefault(candidate =>
            string.Equals(
                ExternalMcpNameSanitizer.SanitizeSegment(candidate.Name, "server"),
                serverName,
                StringComparison.Ordinal));
    }

    private IEnumerable<ExternalMcpServerOptions> EnabledServers()
    {
        return options.Value.Servers.Where(static server => server.Enabled);
    }
}
