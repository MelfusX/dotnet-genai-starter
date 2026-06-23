# MCP Host

`GenAIPlatform.Mcp` is a local stdio Model Context Protocol host for the starter kit. It gives AI clients such as Claude Desktop and Claude Code a fourth consumption surface over the same backend modules used by REST, Worker and the Evaluations CLI.

The host composes explicit Application modules and Infrastructure adapters. MCP tools call Application use cases through `IApplicationDispatcher`, so tenant, user, retrieval and tool-policy behavior stays in the backend. The host is consumer-only: it does not upload documents, mutate document state or expose arbitrary tool execution.

## Service Identity

The local stdio host runs as a configured service identity:

```json
{
  "GenAIPlatform": {
    "Mcp": {
      "Identity": {
        "UserId": "mcp-user",
        "TenantId": "local",
        "Roles": [ "developer" ],
        "Groups": []
      }
    }
  }
}
```

Application handlers see this identity through `IUserContext` and `IBackgroundUserContext`. Listing tools is not the security boundary; handlers still enforce authorization, retrieval filters and tool policy. Per-caller remote MCP authentication is future scope and is not part of the local v0.2.0 host.

## Tools

- `server_info`: returns host version and active service identity details.
- `rag_answer`: answers a question with existing permission-aware RAG retrieval. It preserves the normal no-context fallback, citation behavior and access filters before prompt construction.
- `get_usage`: returns tenant-scoped AI request usage totals with the same Application usage rules as other hosts.
- `get_current_user_profile`: calls the governed Agentic tool use case for the built-in safe profile tool and writes a row to `genai.tool_audit_logs`.

There is no generic `execute_tool_by_name`, `run_tool` or registry executor exposed through MCP. `list_documents` is not exposed because the starter kit does not currently have a read-only user document list use case.

## Approval Limitation

The v0.2.0 MCP host surface supports only safe tools. Approval-required tools are intentionally outside the host surface because there is not yet a broadly supported standard interactive approval flow for MCP clients. If a `RequiresApproval = true` tool is invoked through the direct Application governed-tool use case without approval, it fails closed with `approval_required` and writes an audit row; the MCP host does not expose a path to successfully execute it.

Future protocol support for interactive elicitation or approvals may change this, but the current host keeps approval-required tools out of the MCP tool list.

## Run The Built Host

Build the host first. Do not use `dotnet run` from an MCP client because build output can pollute stdout; MCP stdio stdout must contain protocol frames only. Host logs are configured for stderr.

```powershell
dotnet build src\GenAIPlatform.Mcp\GenAIPlatform.Mcp.csproj
$env:ConnectionStrings__GenAIPlatform = "Host=localhost;Port=5432;Database=genai_platform;Username=genai;Password=genai_dev_password"
dotnet .\src\GenAIPlatform.Mcp\bin\Debug\net10.0\GenAIPlatform.Mcp.dll
```

The RAG and usage tools need the same PostgreSQL database used by the API and Worker. For the local demo, start PostgreSQL first:

```powershell
docker compose up -d postgres
```

## Claude Desktop Configuration

Example `claude_desktop_config.json` entry:

```json
{
  "mcpServers": {
    "genai-platform": {
      "command": "dotnet",
      "args": [
        "E:\\git_repo\\dotnet-genai-starter\\src\\GenAIPlatform.Mcp\\bin\\Debug\\net10.0\\GenAIPlatform.Mcp.dll"
      ],
      "env": {
        "ConnectionStrings__GenAIPlatform": "Host=localhost;Port=5432;Database=genai_platform;Username=genai;Password=genai_dev_password"
      }
    }
  }
}
```

Use the built DLL path from your local checkout. Keep provider API keys out of this file unless you intentionally override the default mock providers for a local experiment.