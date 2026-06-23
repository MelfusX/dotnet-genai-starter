# Changelog

## v0.2.0

Adds a local stdio MCP host as a fourth consumption surface over the existing
Application modules. The host exposes bounded read-only tools for server info,
permission-aware RAG answers, usage totals and the governed current-user profile
tool.

The governed MCP tool calls the Agentic direct tool-execution use case, so safe
tool execution still goes through backend policy and writes `tool_audit_logs`.
Approval-required tools are not exposed as successful MCP host actions; direct
use-case calls without approval fail closed with `approval_required` and are
audited.

Documentation now covers the MCP host, Claude Desktop configuration, safe-only
host limitation and the internal dispatcher swap path if a team chooses MediatR
in its own application.

Repository maintenance is also documented: changes land on protected `main`
through pull requests, public release PRs update `VERSION` and release notes,
and the `publish-release` workflow runs automatically after that `VERSION`
change reaches `main`. It tags the current `main` HEAD and publishes the GitHub
release from the matching `docs/release-notes` file. See `docs/versioning.md`
and `AGENTS.md`.

Not a production system.

## v0.1.0

Initial public reference release.

A .NET 10 starter kit for the backend layer around GenAI applications: RAG over
PostgreSQL + pgvector, model gateway abstraction, prompt versioning, permission-aware
retrieval, sanitized AI request logging, usage/cost tracking, API and CLI evaluations,
and bounded agentic chat with backend-controlled tools.

Modular Clean Architecture: `Domain`, Application modules
(`Core` / `Knowledge` / `Generation` / `Agentic` / `Evaluations` / `Usage`),
`Infrastructure`, and host projects (`Api` / `Worker` / `Evaluations` CLI).
Deterministic mock providers by default; OpenAI-compatible adapters behind ports.

Not a production system.
