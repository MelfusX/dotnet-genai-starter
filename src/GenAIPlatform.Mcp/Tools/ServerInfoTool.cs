using System.ComponentModel;
using GenAIPlatform.Application.Core.Configuration;
using GenAIPlatform.Application.Core.Security;
using GenAIPlatform.Mcp.Tools.Schemas;
using Microsoft.Extensions.Options;
using ModelContextProtocol.Server;

namespace GenAIPlatform.Mcp.Tools;

[McpServerToolType]
public sealed class ServerInfoTool(IUserContext userContext, IOptions<ApplicationOptions> applicationOptions)
{
    [McpServerTool(Name = "server_info", ReadOnly = true, Idempotent = true, OpenWorld = false)]
    [Description("Returns MCP host version and active service identity details.")]
    public string GetServerInfo()
    {
        var response = new ServerInfoResponse(
            applicationOptions.Value.RunnerVersion,
            userContext.UserId,
            userContext.TenantId,
            userContext.Roles,
            userContext.Groups);

        return ServerInfoFormatter.ToMarkdown(response);
    }
}
