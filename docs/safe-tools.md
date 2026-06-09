# Safe Tools

Tool execution is controlled by backend policy. The model can propose actions, but deterministic backend code decides whether they run.

## Flow

```text
LLM proposes tool call
-> backend validates schema
-> policy layer checks permissions and risk
-> safe tool executes automatically
-> risky tool requires approval
-> forbidden tool is rejected
-> audit log is written
```

## Implemented Demo Tools

- `GetCurrentUserProfile` (`v1`): read-only current demo user profile.
- `CreateSupportTicket` (`v1`): creates an idempotent demo ticket payload.
- `DraftEmail` (`v1`): creates a draft payload only; it never sends email.

Each registered tool owns both its model-facing definition and backend policy
metadata. Adding or renaming a registered tool requires updating that single
tool object with its name, schema version, risk and approval behavior. The
policy layer reads metadata from the registered tool instance, and still
fails closed for unknown names or explicitly forbidden tool names.

## Demo Policy

- `GetCurrentUserProfile`: allowed.
- `CreateSupportTicket`: allowed.
- `DraftEmail`: risky; requires simulated approval and still creates a draft only.
- `SendEmail`: forbidden/not implemented.
- `DeleteDocument`: forbidden for LLM.
- `RunSqlQuery`: forbidden.

The model can propose tool calls through the model gateway, but execution is
always controlled by backend code. Unknown and forbidden tools are rejected
deterministically. Tool input validation runs before any allowed execution and
sanitizes the argument payload used by the executor.

`IAgentTool.ExecuteAsync` implementations may report only `Succeeded` or
`Failed`. Pipeline states such as `Rejected`, `ValidationFailed`,
`ApprovalRequired` and `NotExecuted` are produced by backend validation, policy
and loop control before or around tool execution. If a tool implementation
returns one of those pipeline-only statuses from `ExecuteAsync`, the executor
records a failed result with `tool_unexpected_execution_status` so contract
misuse is visible in both the response and audit log.

## Audit Log

Tool audit records are stored in `genai.tool_audit_logs`.

Stored fields include:

- conversation ID;
- user and tenant;
- correlation ID;
- tool call ID;
- tool name;
- schema version;
- policy version;
- validation status;
- policy decision;
- approval state;
- execution status;
- sanitized arguments;
- output metadata;
- optional error code/message;
- created timestamp.

## Requirements

- Tool schemas are typed.
- Tool schemas are versioned.
- Tool input is validated.
- Tool execution is logged.
- Dangerous tools require approval or are forbidden.
- The LLM receives no infrastructure credentials.
- Demo tools are side-effect-limited; `DraftEmail` creates a draft payload only and never sends email.
