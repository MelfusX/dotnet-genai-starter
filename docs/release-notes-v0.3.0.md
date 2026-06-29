# v0.3.0 Reference Release Notes

`v0.3.0` adds MCP client support for external stdio MCP servers. It keeps the starter-kit framing from earlier releases: this is a reference implementation, not a production deployment or stable framework.

## Included

- An Application Agentic port for external tool sources plus a composite registry that lists built-in tools first and then external tools.
- An Infrastructure MCP client adapter for configured stdio servers under `GenAIPlatform:ExternalMcp:Servers`.
- Server configuration for `Name`, `Enabled`, `Command`, `Arguments`, `WorkingDirectory`, `AllowedTools`, `ConnectTimeoutSeconds` and `ToolCallTimeoutSeconds`.
- Backend-owned snapshots of external tool definitions at connect time, with snapshot hashes used as tool schema/audit provenance.
- Sanitized, provider-safe external tool names in the form `mcp_<server>_<tool>`.
- Sanitized external tool descriptions and JSON round-trip argument mapping that preserves nested shape and numeric fidelity (via System.Text.Json) at the adapter boundary.
- Approval-required policy for every external MCP tool by default.
- Agentic governance coverage for external tools: validation, policy, approval, budget and tool audit use the same path as built-in tools.
- Resilient connection lifecycle: non-blocking startup, bounded-parallel connect that preserves the deterministic tool listing, a background refresh pass that recovers servers unavailable at startup, and a per-server connection-policy seam. Configurable through `ConnectOnStartup`, `MaxParallelConnects` and `RefreshInterval`.

## Safety And Governance Defaults

- No external MCP server is configured by default.
- Only configured and enabled servers are considered. Per-server `AllowedTools` can narrow the exposed tool set.
- External tool descriptions and schemas are treated as untrusted server input, not as a security boundary.
- A server cannot silently replace the audited tool definition after connect: the agent sees the captured snapshot and audit provenance uses the snapshot hash.
- Unavailable or timed-out servers degrade or fail closed instead of bypassing approval or crashing the agentic loop.
- External MCP tools are consumed by the platform's agentic loop; they are not exposed through the local MCP host as a generic `run any tool` executor.

## Known Non-goals

- Production-ready remote MCP authentication, secret storage and enterprise connector management are not included.
- Circuit-breaker backoff escalation, jitter and capability-down-as-evidence reporting are future work; the connection-policy seam always permits attempts in this release.
- The local MCP host still does not expose approval-required tools as successful host actions.
- The automated release gate does not launch real child-process MCP servers such as `npx`; deterministic fake sources and adapter fakes cover the behavior.
- This release does not make the starter kit production-ready.

## Verification Scope

The local release gate for this changeset includes restore, build, full unit and Docker-backed integration tests, formatting verification, code-organization gate, package-vulnerability gate and whitespace checking.

Coverage added for the external MCP client path verifies composite registry ordering, server/tool allow-list behavior, sanitized prefixes/descriptions, snapshot-hash provenance, JSON argument fidelity, timeout and cancellation outcomes, timeout-to-unavailable behavior, unavailable-server degradation, approval-required execution, blacklist-before-wrapper-policy rejection and rug-pull behavior. Connection-lifecycle coverage adds bounded-parallel connect with preserved listing order, recovery of a server that was unavailable at startup, refresh leaving available servers untouched, non-blocking startup, connect-on-startup skip and policy-denied connect.

## Suggested Tag Notes

```text
v0.3.0 - external MCP tools under first-party governance

Adds MCP client support for configured external stdio MCP servers. External tools are surfaced in the platform's agentic loop with backend-owned snapshots, mcp_<server>_<tool> prefixes, approval-required policy and normal tool audit provenance.

No external server is configured by default, and production remote auth/secrets remain future adapter work.
```
