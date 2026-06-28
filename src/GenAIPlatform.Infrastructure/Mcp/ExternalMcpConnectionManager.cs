using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GenAIPlatform.Infrastructure.Mcp;

internal sealed class ExternalMcpConnectionManager(
    IOptions<ExternalMcpOptions> options,
    IExternalMcpClientFactory clientFactory,
    ILogger<ExternalMcpConnectionManager> logger) : IExternalMcpConnectionManager, IHostedService, IDisposable, IAsyncDisposable
{
    private readonly ExternalMcpConnectionState state = new();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var connected = new List<ExternalMcpServerSnapshot>();
        var servers = EnabledServers().ToArray();

        for (var index = 0; index < servers.Length; index++)
        {
            connected.Add(await ConnectAndSnapshotAsync(servers[index], index, cancellationToken));
        }

        state.ReplaceSnapshots(connected);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await DisposeConnectionsAsync();
    }

    public IReadOnlyList<ExternalMcpServerSnapshot> GetSnapshots()
    {
        return state.GetSnapshots();
    }

    public async Task<ExternalMcpToolCallResult> CallToolAsync(
        ExternalMcpToolSnapshot tool,
        IReadOnlyDictionary<string, object?>? arguments,
        CancellationToken cancellationToken)
    {
        var connection = await GetOrReconnectAsync(tool.ServerName, cancellationToken);
        if (connection is null)
        {
            return ExternalMcpToolCallResult.Unavailable("External MCP server is unavailable.");
        }

        var firstAttempt = await TryCallAsync(connection, tool, arguments, cancellationToken);
        if (firstAttempt is not null)
        {
            return firstAttempt;
        }

        connection = await GetOrReconnectAsync(tool.ServerName, cancellationToken);
        if (connection is null)
        {
            return ExternalMcpToolCallResult.Unavailable("External MCP server is unavailable after reconnect.");
        }

        var secondAttempt = await TryCallAsync(connection, tool, arguments, cancellationToken);
        return secondAttempt ?? ExternalMcpToolCallResult.Unavailable("External MCP tool call failed after reconnect.");
    }

    public void Dispose()
    {
        DisposeConnectionsAsync().GetAwaiter().GetResult();
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeConnectionsAsync();
    }

    private async Task<ExternalMcpToolCallResult?> TryCallAsync(
        ExternalMcpServerConnection connection,
        ExternalMcpToolSnapshot tool,
        IReadOnlyDictionary<string, object?>? arguments,
        CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(tool.ToolCallTimeout);

        try
        {
            return await connection.Client.CallToolAsync(tool.OriginalName, arguments, timeout.Token);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException exception)
        {
            logger.LogWarning(exception, "External MCP tool call timed out for server {ServerName}.", tool.ServerName);
            await MarkUnavailableAsync(tool.ServerName);
            return ExternalMcpToolCallResult.Unavailable(
                "External MCP tool execution timed out.",
                "mcp_tool_timeout");
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "External MCP tool call failed for server {ServerName}.", tool.ServerName);
            await MarkUnavailableAsync(tool.ServerName);
            return null;
        }
    }

    private async Task<ExternalMcpServerSnapshot> ConnectAndSnapshotAsync(
        ExternalMcpServerOptions server,
        int order,
        CancellationToken cancellationToken)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(server.ConnectTimeoutSeconds));

        IExternalMcpClient? client = null;
        try
        {
            client = await clientFactory.CreateAsync(server, timeout.Token);
            var descriptors = await client.ListToolsAsync(timeout.Token);
            var snapshot = ExternalMcpSnapshotBuilder.Build(server, order, descriptors, isAvailable: true);
            state.SetConnection(snapshot.ServerName, new ExternalMcpServerConnection(server, client));
            client = null;
            return snapshot;
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            if (client is not null)
            {
                await client.DisposeAsync();
            }

            logger.LogWarning(exception, "External MCP server {ServerName} is unavailable.", server.Name);
            return new ExternalMcpServerSnapshot(
                ExternalMcpNameSanitizer.SanitizeSegment(server.Name, "server"),
                order,
                IsAvailable: false,
                []);
        }
    }

    private async Task<ExternalMcpServerConnection?> GetOrReconnectAsync(
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

        try
        {
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(TimeSpan.FromSeconds(server.ConnectTimeoutSeconds));
            var client = await clientFactory.CreateAsync(server, timeout.Token);
            connection = new ExternalMcpServerConnection(server, client);
            state.SetConnection(serverName, connection);
            state.MarkAvailability(serverName, isAvailable: true);
            return connection;
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            logger.LogWarning(exception, "External MCP server {ServerName} reconnect failed.", server.Name);
            state.MarkAvailability(serverName, isAvailable: false);
            return null;
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

    private async Task MarkUnavailableAsync(string serverName)
    {
        var connection = state.RemoveConnection(serverName);
        state.MarkAvailability(serverName, isAvailable: false);
        if (connection is not null)
        {
            await connection.DisposeAsync();
        }
    }

    private async Task DisposeConnectionsAsync()
    {
        foreach (var connection in state.ClearConnections())
        {
            await connection.DisposeAsync();
        }
    }
}