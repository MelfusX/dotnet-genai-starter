using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GenAIPlatform.Infrastructure.Mcp;

internal sealed class ExternalMcpConnectionManager(
    IOptions<ExternalMcpOptions> options,
    IExternalMcpClientFactory clientFactory,
    IExternalMcpConnectionPolicy policy,
    ILogger<ExternalMcpConnectionManager> logger) : IExternalMcpConnectionManager, IHostedService, IDisposable, IAsyncDisposable
{
    private readonly ExternalMcpConnectionState state = new();
    private readonly ExternalMcpConnector connector = new(options, clientFactory, policy, logger);
    private readonly CancellationTokenSource lifetime = new();
    private Task background = Task.CompletedTask;
    private int disposed;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Non-blocking: warmup and periodic recovery run in the background so a slow or
        // unreachable server never delays host startup.
        var refresher = new ExternalMcpBackgroundRefresher(connector, state, options, logger);
        background = Task.Run(() => refresher.RunAsync(lifetime.Token), CancellationToken.None);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await lifetime.CancelAsync();
        await WaitForBackgroundAsync(cancellationToken);
        await DisposeConnectionsAsync();
    }

    /// <summary>Background warmup/recovery task, exposed for deterministic tests.</summary>
    internal Task BackgroundActivity => background;

    public IReadOnlyList<ExternalMcpServerSnapshot> GetSnapshots()
    {
        return state.GetSnapshots();
    }

    public Task RefreshAsync(CancellationToken cancellationToken)
    {
        return connector.RefreshAsync(state, cancellationToken);
    }

    public async Task<ExternalMcpToolCallResult> CallToolAsync(
        ExternalMcpToolSnapshot tool,
        IReadOnlyDictionary<string, object?>? arguments,
        CancellationToken cancellationToken)
    {
        var connection = await connector.GetOrReconnectAsync(state, tool.ServerName, cancellationToken);
        if (connection is null)
        {
            return ExternalMcpToolCallResult.Unavailable("External MCP server is unavailable.");
        }

        var firstAttempt = await TryCallAsync(connection, tool, arguments, cancellationToken);
        if (firstAttempt is not null)
        {
            return firstAttempt;
        }

        connection = await connector.GetOrReconnectAsync(state, tool.ServerName, cancellationToken);
        if (connection is null)
        {
            return ExternalMcpToolCallResult.Unavailable("External MCP server is unavailable after reconnect.");
        }

        var secondAttempt = await TryCallAsync(connection, tool, arguments, cancellationToken);
        return secondAttempt ?? ExternalMcpToolCallResult.Unavailable("External MCP tool call failed after reconnect.");
    }

    public void Dispose()
    {
        // The manager is tracked once as the concrete singleton and once via the interface
        // factory, so the container disposes it twice; make disposal idempotent.
        if (Interlocked.Exchange(ref disposed, 1) == 1)
        {
            return;
        }

        lifetime.Cancel();
        try
        {
            background.GetAwaiter().GetResult();
        }
        catch (OperationCanceledException)
        {
        }

        DisposeConnectionsAsync().GetAwaiter().GetResult();
        lifetime.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref disposed, 1) == 1)
        {
            return;
        }

        await lifetime.CancelAsync();
        await WaitForBackgroundAsync(CancellationToken.None);
        await DisposeConnectionsAsync();
        lifetime.Dispose();
    }

    private async Task WaitForBackgroundAsync(CancellationToken cancellationToken)
    {
        try
        {
            await background.WaitAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
        }
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

    private async Task MarkUnavailableAsync(string serverName)
    {
        var connection = state.RemoveConnection(serverName);
        state.MarkStatus(serverName, ExternalMcpServerStatus.Unavailable);
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
