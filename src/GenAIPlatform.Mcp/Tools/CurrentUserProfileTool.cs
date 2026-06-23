using System.ComponentModel;
using System.Text.Json;
using GenAIPlatform.Application.Agentic.Tools.Execute;
using GenAIPlatform.Application.Core.Dispatching;
using GenAIPlatform.Application.Core.Exceptions;
using GenAIPlatform.Domain.Agentic;
using GenAIPlatform.Mcp.Tools.Schemas;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace GenAIPlatform.Mcp.Tools;

[McpServerToolType]
public sealed class CurrentUserProfileTool(IApplicationDispatcher dispatcher)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [McpServerTool(Name = "get_current_user_profile", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Returns the current MCP service identity through backend-governed tool policy and audit.")]
    public async Task<string> GetCurrentUserProfileAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var arguments = JsonDocument.Parse("{}");
            var response = await dispatcher.DispatchAsync<ExecuteToolCommand, ExecuteToolResponse>(
                new ExecuteToolCommand(
                    "GetCurrentUserProfile",
                    arguments.RootElement.Clone()),
                cancellationToken);

            if (response.ExecutionStatus != ToolExecutionStatus.Succeeded ||
                string.IsNullOrWhiteSpace(response.Result))
            {
                throw new McpException(
                    $"get_current_user_profile failed: {response.ErrorCode ?? response.ExecutionStatus.ToString()}");
            }

            var profile = JsonSerializer.Deserialize<CurrentUserProfileResponse>(
                response.Result,
                JsonOptions);

            return CurrentUserProfileFormatter.ToMarkdown(profile ?? CurrentUserProfileResponse.Empty);
        }
        catch (AppException exception)
        {
            throw new McpException($"get_current_user_profile failed: {exception.Message}");
        }
    }
}
