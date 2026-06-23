# v0.2.0 Reference Release Notes

`v0.2.0` adds a local MCP host to the .NET-native GenAI Platform Starter Kit. It keeps the starter-kit framing from `v0.1.0`: this is a reference implementation, not a production deployment or stable framework.

## Included

- `GenAIPlatform.Mcp`, a local stdio MCP host built on the official C# MCP SDK.
- MCP tools over existing backend use cases: `server_info`, `rag_answer`, `get_usage` and `get_current_user_profile`.
- Service-identity based MCP composition through `GenAIPlatform:Mcp:Identity`.
- `get_current_user_profile` as one concrete governed Agentic tool exposed through MCP. It calls the backend direct tool-execution use case and writes `genai.tool_audit_logs`.
- Direct governed-tool behavior for approval-required tools remains fail-closed without approval and still writes audit records.
- MCP documentation with built-binary startup guidance and a `claude_desktop_config.json` example using `ConnectionStrings__GenAIPlatform`.
- Documentation for the internal dispatcher to MediatR swap boundary.

## Repository Maintenance

- Contribution and release flow is documented in `docs/versioning.md` and `AGENTS.md`: protected
  `main`, pull-request-only changes, agent branch pushes only after explicit maintainer approval,
  and release-ready public PRs with an explicit `VERSION` update.
- The `publish-release` workflow runs automatically after a release PR changes `VERSION` on `main`,
  reads `VERSION`, reruns build, formatting, code-organization, vulnerability and unit-test gates,
  then tags the current `main` HEAD and publishes from the matching release-notes file. Full
  integration coverage remains a PR/main CI responsibility.

## Safety And Privacy Defaults

- The MCP host is consumer-only and does not expose document upload, document mutation or arbitrary registry execution.
- There is no generic `execute_tool_by_name` or `run_tool` MCP capability.
- The MCP host surface supports only safe tools in v0.2.0. Approval-required tools are outside the host surface until interactive approval support is explicit across clients and protocol usage.
- RAG behavior keeps the normal access filters, no-context fallback and citation semantics before any context reaches a model prompt.
- Full rendered prompt logging remains disabled by default.
- Automated tests continue to use mock providers by default.

## Known Non-goals

- Remote, multi-tenant MCP authentication and caller-specific credentials are future scope.
- MCP client support for consuming external MCP servers is not included in v0.2.0.
- Interactive approval flows for MCP-hosted risky tools are not implemented.
- This release does not make the starter kit production-ready.

## Verification Scope

The local release gate for this tag included restore, build, full unit and Docker-backed integration tests, formatting verification, code-organization gate, package-vulnerability gate and whitespace checking. Integration coverage verifies that `get_current_user_profile` through the MCP host wrapper returns the configured MCP identity and writes a tool audit row, and that a direct risky tool call returns `approval_required` and writes a tool audit row.

## Suggested Tag Notes

```text
v0.2.0 - local MCP host release

Adds GenAIPlatform.Mcp as a local stdio MCP host over existing Application use cases, including server info, permission-aware RAG, usage totals and one governed safe Agentic tool with policy + audit.

The host is intentionally safe-only: no generic tool executor, no document mutation and no successful approval-required tool execution path through MCP. Risky direct tool calls fail closed with approval_required and remain audited.
```
