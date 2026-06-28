using GenAIPlatform.Domain.Agentic;
using GenAIPlatform.Infrastructure.Mcp;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace GenAIPlatform.UnitTests;

public sealed class ExternalMcpAgentToolSourceTests
{
    [Fact]
    public void BuildPrefixedToolName_UsesProviderSafeAsciiAndStableMaxLength()
    {
        var name = ExternalMcpNameSanitizer.BuildPrefixedToolName(
            "Админ Server With A Very Very Long Name That Should Be Shortened",
            "Run SQL Query With A Very Very Long Name That Should Also Be Shortened");

        Assert.True(name.Length <= ExternalMcpNameSanitizer.MaxToolNameLength);
        Assert.Matches("^[a-z0-9_]+$", name);
        Assert.StartsWith("mcp_server_with_a_very", name, StringComparison.Ordinal);
    }
    [Fact]
    public async Task GetAvailableTools_UsesConfigOrderToolSortAllowListAndSafeMetadata()
    {
        var factory = new FakeExternalMcpClientFactory();
        factory.SetClient("Beta Server", FakeExternalMcpClient.WithTools(
            Tool("zeta", "ignored", "{}"),
            Tool("A Tool", "line\r\nwith\tcontrols", Schema())));
        factory.SetClient("Alpha", FakeExternalMcpClient.WithTools(
            Tool("allowed", new string('x', ExternalMcpDescriptionSanitizer.MaxLength + 20), Schema()),
            Tool("blocked", "not allow-listed", Schema())));
        var manager = CreateManager(factory,
            Server("Beta Server"),
            Server("Alpha", allowedTools: ["allowed"]));

        await manager.StartAsync(CancellationToken.None);

        var tools = new ExternalMcpAgentToolSource(manager).GetAvailableTools();

        Assert.Equal(["mcp_beta_server_a_tool", "mcp_beta_server_zeta", "mcp_alpha_allowed"],
            tools.Select(static tool => tool.Definition.Name).ToArray());
        Assert.All(tools, static tool =>
        {
            Assert.Equal(ToolRisk.Risky, tool.Policy.Risk);
            Assert.True(tool.Policy.RequiresApproval);
            Assert.StartsWith("sha256:", tool.Definition.SchemaVersion, StringComparison.Ordinal);
        });
        Assert.DoesNotContain('\r', tools[0].Definition.Description);
        Assert.DoesNotContain('\n', tools[0].Definition.Description);
        Assert.True(tools[2].Definition.Description.Length <= ExternalMcpDescriptionSanitizer.MaxLength);
    }

    [Fact]
    public async Task GetAvailableTools_KeepsDefinitionSnapshotCapturedAtConnect()
    {
        var client = FakeExternalMcpClient.WithTools(Tool("echo", "first definition", Schema("first")));
        var factory = new FakeExternalMcpClientFactory();
        factory.SetClient("Server", client);
        var manager = CreateManager(factory, Server("Server"));
        await manager.StartAsync(CancellationToken.None);
        var source = new ExternalMcpAgentToolSource(manager);
        var first = Assert.Single(source.GetAvailableTools()).Definition;

        client.Tools = [Tool("echo", "changed definition", Schema("changed"))];
        var second = Assert.Single(source.GetAvailableTools()).Definition;

        Assert.Equal("first definition", second.Description);
        Assert.Equal(first.SchemaVersion, second.SchemaVersion);
        Assert.Contains("first", second.InputSchema.GetRawText(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task StartAsync_DisposesClientWhenToolSnapshotListingFails()
    {
        var client = FakeExternalMcpClient.WithTools(Tool("echo", "Echoes input.", Schema()));
        client.ListToolsAsyncOverride = _ => throw new InvalidOperationException("list failed");
        var factory = new FakeExternalMcpClientFactory();
        factory.SetClient("Server", client);
        var manager = CreateManager(factory, Server("Server"));

        await manager.StartAsync(CancellationToken.None);

        Assert.True(client.Disposed);
        Assert.Empty(new ExternalMcpAgentToolSource(manager).GetAvailableTools());
    }

    [Fact]
    public async Task ExecuteAsync_RoundTripsNestedJsonArgumentsToSdkShape()
    {
        var client = FakeExternalMcpClient.WithTools(Tool("echo", "Echoes input.", Schema()));
        client.CallResult = new ExternalMcpToolCallResult(
            IsError: false,
            JsonSerializer.SerializeToElement(new { ok = true }),
            ErrorMessage: null);
        var factory = new FakeExternalMcpClientFactory();
        factory.SetClient("Server", client);
        var manager = CreateManager(factory, Server("Server"));
        await manager.StartAsync(CancellationToken.None);
        var tool = Assert.Single(new ExternalMcpAgentToolSource(manager).GetAvailableTools());
        using var arguments = JsonDocument.Parse("""
        {
          "text": "hello",
          "nested": { "count": 3, "flags": [ true, false ], "missing": null },
          "items": [ { "name": "a" }, { "name": "b" } ]
        }
        """);

        var validation = tool.Validate(arguments.RootElement);
        var result = await tool.ExecuteAsync(validation.SanitizedArguments, CancellationToken.None);

        Assert.Equal(ToolExecutionStatus.Succeeded, result.Status);
        var captured = JsonSerializer.SerializeToElement(client.CapturedArguments);
        Assert.Equal("hello", captured.GetProperty("text").GetString());
        Assert.Equal(3, captured.GetProperty("nested").GetProperty("count").GetInt32());
        Assert.True(captured.GetProperty("nested").GetProperty("flags")[0].GetBoolean());
        Assert.Equal(JsonValueKind.Null, captured.GetProperty("nested").GetProperty("missing").ValueKind);
        Assert.Equal("b", captured.GetProperty("items")[1].GetProperty("name").GetString());
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsFailedOnTimeoutAndMarksServerUnavailable()
    {
        var client = FakeExternalMcpClient.WithTools(Tool("slow", "Slow tool.", Schema()));
        client.CallAsync = async (_, _, cancellationToken) =>
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
            return new ExternalMcpToolCallResult(false, JsonSerializer.SerializeToElement(new { ok = true }), null);
        };
        var factory = new FakeExternalMcpClientFactory();
        factory.SetClient("Server", client);
        var manager = CreateManager(factory, Server("Server", timeoutSeconds: 0.01));
        await manager.StartAsync(CancellationToken.None);
        var tool = Assert.Single(new ExternalMcpAgentToolSource(manager).GetAvailableTools());

        var timeout = await tool.ExecuteAsync(EmptyObject(), CancellationToken.None);

        Assert.Equal(ToolExecutionStatus.Failed, timeout.Status);
        Assert.Equal("mcp_tool_timeout", timeout.ErrorCode);
        Assert.True(client.Disposed);
        Assert.Empty(new ExternalMcpAgentToolSource(manager).GetAvailableTools());
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsFailedOnCallerCancellation()
    {
        var client = FakeExternalMcpClient.WithTools(Tool("slow", "Slow tool.", Schema()));
        client.CallAsync = async (_, _, cancellationToken) =>
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, cancellationToken);
            return new ExternalMcpToolCallResult(false, JsonSerializer.SerializeToElement(new { ok = true }), null);
        };
        var factory = new FakeExternalMcpClientFactory();
        factory.SetClient("Server", client);
        var manager = CreateManager(factory, Server("Server", timeoutSeconds: 30));
        await manager.StartAsync(CancellationToken.None);
        var tool = Assert.Single(new ExternalMcpAgentToolSource(manager).GetAvailableTools());
        using var canceled = new CancellationTokenSource();
        await canceled.CancelAsync();

        var cancellation = await tool.ExecuteAsync(EmptyObject(), canceled.Token);

        Assert.Equal(ToolExecutionStatus.Failed, cancellation.Status);
        Assert.Equal("mcp_tool_canceled", cancellation.ErrorCode);
    }
    [Fact]
    public async Task ExecuteAsync_ReturnsFailedForRemoteErrorAndUnavailableServerIsNotListed()
    {
        var unavailableFactory = new FakeExternalMcpClientFactory();
        var unavailableManager = CreateManager(unavailableFactory, Server("Missing"));
        await unavailableManager.StartAsync(CancellationToken.None);

        Assert.Empty(new ExternalMcpAgentToolSource(unavailableManager).GetAvailableTools());

        var client = FakeExternalMcpClient.WithTools(Tool("fails", "Fails remotely.", Schema()));
        client.CallResult = new ExternalMcpToolCallResult(
            IsError: true,
            JsonSerializer.SerializeToElement(new { failed = true }),
            "remote failure");
        var factory = new FakeExternalMcpClientFactory();
        factory.SetClient("Server", client);
        var manager = CreateManager(factory, Server("Server"));
        await manager.StartAsync(CancellationToken.None);
        var tool = Assert.Single(new ExternalMcpAgentToolSource(manager).GetAvailableTools());

        var result = await tool.ExecuteAsync(EmptyObject(), CancellationToken.None);

        Assert.Equal(ToolExecutionStatus.Failed, result.Status);
        Assert.Equal("mcp_tool_error", result.ErrorCode);
        Assert.Equal("remote failure", result.ErrorMessage);
    }

    [Fact]
    public async Task ExecuteAsync_ReconnectsAfterFailedCallAndStopDisposesConnections()
    {
        var firstClient = FakeExternalMcpClient.WithTools(Tool("echo", "Echoes input.", Schema()));
        firstClient.CallAsync = (_, _, _) => throw new InvalidOperationException("server died");
        var secondClient = FakeExternalMcpClient.WithTools(Tool("echo", "Echoes input.", Schema()));
        secondClient.CallResult = new ExternalMcpToolCallResult(
            IsError: false,
            JsonSerializer.SerializeToElement(new { reconnected = true }),
            ErrorMessage: null);
        var factory = new FakeExternalMcpClientFactory();
        factory.EnqueueClient("Server", firstClient);
        factory.EnqueueClient("Server", secondClient);
        var manager = CreateManager(factory, Server("Server"));
        await manager.StartAsync(CancellationToken.None);
        var tool = Assert.Single(new ExternalMcpAgentToolSource(manager).GetAvailableTools());

        var result = await tool.ExecuteAsync(EmptyObject(), CancellationToken.None);
        await manager.StopAsync(CancellationToken.None);

        Assert.Equal(ToolExecutionStatus.Succeeded, result.Status);
        Assert.True(result.Output.GetProperty("reconnected").GetBoolean());
        Assert.Equal(2, factory.CreateCount("Server"));
        Assert.True(firstClient.Disposed);
        Assert.True(secondClient.Disposed);
        Assert.Empty(new ExternalMcpAgentToolSource(manager).GetAvailableTools());
    }

    private static ExternalMcpConnectionManager CreateManager(
        FakeExternalMcpClientFactory factory,
        params ExternalMcpServerOptions[] servers)
    {
        return new ExternalMcpConnectionManager(
            Options.Create(new ExternalMcpOptions { Servers = servers.ToList() }),
            factory,
            NullLogger<ExternalMcpConnectionManager>.Instance);
    }

    private static ExternalMcpServerOptions Server(
        string name,
        IReadOnlyCollection<string>? allowedTools = null,
        double timeoutSeconds = 30)
    {
        return new ExternalMcpServerOptions
        {
            Name = name,
            Command = "fake",
            AllowedTools = allowedTools?.ToList() ?? [],
            ToolCallTimeoutSeconds = timeoutSeconds
        };
    }

    private static ExternalMcpToolDescriptor Tool(string name, string? description, string schema)
    {
        using var document = JsonDocument.Parse(schema);
        return new ExternalMcpToolDescriptor(name, description, document.RootElement.Clone());
    }

    private static string Schema(string marker = "value") => $$"""
    {
      "type": "object",
      "properties": {
        "{{marker}}": { "type": "string" }
      },
      "additionalProperties": true
    }
    """;

    private static JsonElement EmptyObject()
    {
        using var document = JsonDocument.Parse("{}");
        return document.RootElement.Clone();
    }

    private sealed class FakeExternalMcpClientFactory : IExternalMcpClientFactory
    {
        private readonly Dictionary<string, Queue<IExternalMcpClient>> clients = new(StringComparer.Ordinal);
        private readonly Dictionary<string, int> createCounts = new(StringComparer.Ordinal);

        public void SetClient(string serverName, IExternalMcpClient client)
        {
            clients[serverName] = new Queue<IExternalMcpClient>([client]);
        }

        public void EnqueueClient(string serverName, IExternalMcpClient client)
        {
            if (!clients.TryGetValue(serverName, out var queue))
            {
                queue = new Queue<IExternalMcpClient>();
                clients[serverName] = queue;
            }

            queue.Enqueue(client);
        }

        public int CreateCount(string serverName) => createCounts.GetValueOrDefault(serverName);

        public Task<IExternalMcpClient> CreateAsync(
            ExternalMcpServerOptions server,
            CancellationToken cancellationToken)
        {
            createCounts[server.Name] = createCounts.GetValueOrDefault(server.Name) + 1;
            if (!clients.TryGetValue(server.Name, out var queue))
            {
                throw new InvalidOperationException("No fake MCP client was configured.");
            }

            return Task.FromResult(queue.Dequeue());
        }
    }

    private sealed class FakeExternalMcpClient : IExternalMcpClient
    {
        public IReadOnlyList<ExternalMcpToolDescriptor> Tools { get; set; } = [];

        public IReadOnlyDictionary<string, object?>? CapturedArguments { get; private set; }

        public ExternalMcpToolCallResult CallResult { get; set; } = new(
            IsError: false,
            JsonSerializer.SerializeToElement(new { ok = true }),
            ErrorMessage: null);

        public Func<string, IReadOnlyDictionary<string, object?>?, CancellationToken, Task<ExternalMcpToolCallResult>>? CallAsync { get; set; }

        public Func<CancellationToken, Task<IReadOnlyList<ExternalMcpToolDescriptor>>>? ListToolsAsyncOverride { get; set; }

        public bool Disposed { get; private set; }

        public static FakeExternalMcpClient WithTools(params ExternalMcpToolDescriptor[] tools)
        {
            return new FakeExternalMcpClient { Tools = tools };
        }

        public Task<IReadOnlyList<ExternalMcpToolDescriptor>> ListToolsAsync(CancellationToken cancellationToken)
        {
            return ListToolsAsyncOverride is null
                ? Task.FromResult(Tools)
                : ListToolsAsyncOverride(cancellationToken);
        }

        public Task<ExternalMcpToolCallResult> CallToolAsync(
            string toolName,
            IReadOnlyDictionary<string, object?>? arguments,
            CancellationToken cancellationToken)
        {
            CapturedArguments = arguments;
            return CallAsync is null
                ? Task.FromResult(CallResult)
                : CallAsync(toolName, arguments, cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            Disposed = true;
            return ValueTask.CompletedTask;
        }
    }
}