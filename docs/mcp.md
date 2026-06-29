# MCP Host And External Tools

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

## External MCP Tools In Agentic Chat

`v0.3.0` adds MCP client support for consuming external stdio MCP servers from the platform's agentic loop. This is intentionally different from exposing the local MCP host as a generic executor: external MCP tools are adapted inside Infrastructure, registered as Agentic tools through the Application port and then evaluated by the same backend validation, policy, approval, budget and audit path as built-in tools.

No external servers are configured by default. When a server is configured and enabled, the Infrastructure adapter connects to it, lists its tools and creates backend-owned tool wrappers. The final model-facing names are sanitized, provider-safe and prefixed as `mcp_<server>_<tool>`, so an external server cannot shadow a built-in tool name or bypass blacklist policy by choosing a conflicting name.

Configuration lives under `GenAIPlatform:ExternalMcp:Servers`:

```json
{
  "GenAIPlatform": {
    "ExternalMcp": {
      "ConnectOnStartup": true,
      "MaxParallelConnects": 4,
      "RefreshInterval": "00:01:00",
      "Servers": [
        {
          "Name": "local-tools",
          "Enabled": true,
          "Command": "npx",
          "Arguments": [ "-y", "@modelcontextprotocol/server-everything" ],
          "WorkingDirectory": null,
          "AllowedTools": [ "echo" ],
          "ConnectTimeoutSeconds": 10,
          "ToolCallTimeoutSeconds": 30
        }
      ]
    }
  }
}
```

The `Servers` list is the server allow-list: only configured, enabled servers are considered. `AllowedTools` is an optional per-server tool allow-list. When `AllowedTools` is empty, all tools reported by that enabled server are wrapped; when it is populated, only matching original external tool names are exposed to the agentic registry.

Connection lifecycle is controlled at the `ExternalMcp` level. `ConnectOnStartup` (default `true`) runs a startup warmup; set it to `false` to connect on demand instead. `MaxParallelConnects` (default `4`) bounds how many servers connect concurrently so one slow or hung server cannot delay the others, while connect order never changes the deterministic tool listing. `RefreshInterval` (default one minute, `00:00:00` to disable) is a background pass that re-attempts servers which are not currently available and lists their tools; already-available servers are left untouched. Startup is non-blocking: the warmup and recovery run in the background, so an unreachable server never delays host startup.

External tool definitions are treated as untrusted input. Descriptions are sanitized and length-limited before they can enter a model prompt. Tool argument payloads are passed through JSON round-trip conversion at the adapter boundary so nested objects and arrays are preserved.

## External Tool Governance Guarantees

The external MCP adapter keeps governance in the platform:

- Snapshot at connect: tool name, description and input schema are captured when the server connects.
- Snapshot provenance: the snapshot hash becomes the backend-owned tool schema version and is written to tool audit provenance. The model-proposed schema version does not control external-tool audit provenance.
- Prefixed names: external tools are exposed as provider-safe `mcp_<server>_<tool>` names after ASCII sanitization and length limiting.
- Approval by default: every external MCP tool is registered as approval-required, regardless of how the external server describes itself.
- Backend allow-list: only configured servers and allowed tools can appear in the agentic registry.
- Fail closed/degrade: unavailable or timed-out servers produce no available tools or failed tool results rather than bypassing policy or crashing the agentic loop. A server that is unavailable at startup recovers on the next background refresh pass (or an explicit refresh); a connection that drops after a tool was listed is re-established on the next call to that tool.
- Audit path: executed, approval-required, rejected and failed external tool calls go through the same tool audit mechanism as built-in Agentic tools.

This starter-kit release does not claim production-ready remote MCP authentication, secret storage or enterprise connector management. External stdio server configuration is a local/sample adapter pattern; production credential handling and remote multi-tenant MCP are future work.

The main gate uses fake external tool sources and adapter-level fakes for deterministic coverage. It does not rely on real child-process MCP servers such as `npx` during automated tests.

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
