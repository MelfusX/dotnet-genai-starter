# AGENTS.md

Treat this file as the operating contract for future coding agents and human maintainers.

## Project Intent

`dotnet-genai-starter` is a .NET-native GenAI Platform Starter Kit. It demonstrates the backend platform layer around GenAI applications: RAG, model gateway abstraction, prompt versioning, permission-aware retrieval, sanitized AI request logging, cost tracking, evaluations and backend-controlled tool execution.

This is a reference implementation, not a production system and not a stable framework. Keep changes practical, reviewable and aligned with the public documentation.

## First Read

Before making non-trivial changes, read the relevant public docs:

- `README.md` for scope, quickstart and release framing.
- `docs/architecture.md` for project boundaries.
- `docs/code-organization.md` for maintainability rules.
- `docs/security-model.md` for auth, retrieval and logging safety.
- `docs/rag-pipeline.md` for ingestion, retrieval and citation behavior.
- `docs/model-gateway.md` for provider abstraction rules.
- `docs/safe-tools.md` for agentic tool policy.
- `docs/trade-offs.md` for documented compromises.

## Architecture Contract

- Keep the solution a modular monolith.
- `Domain` must not depend on Application modules, `Infrastructure`, `Api`, `Worker`, provider SDKs or persistence libraries.
- Application is split into project-level modules:
  `GenAIPlatform.Application.Core`, `.Knowledge`, `.Generation`, `.Agentic`, `.Evaluations` and `.Usage`.
- `Core` may reference only `Domain`.
- `Knowledge` may reference only `Domain` and `Core`.
- `Generation` may reference only `Domain`, `Core` and `Knowledge`.
- `Agentic` may reference only `Domain`, `Core` and `Generation`.
- `Evaluations` may reference only `Domain`, `Core`, `Knowledge` and `Generation`.
- `Usage` may reference only `Domain` and `Core`.
- Application modules own use cases, ports, orchestration, validation policies and pipeline behavior.
- `Infrastructure` implements persistence, pgvector retrieval, file storage, model clients, embedding clients and other adapters.
- The observability mechanism lives in `Infrastructure`, including sanitized AI request logging, pricing/cost estimation and log/pricing persistence details.
- `Api` maps HTTP input/output, OpenAPI metadata and foreground user context only.
- `Worker` runs background jobs by dispatching Application use cases and should compose only the Application modules it needs.
- `Evaluations` is a CLI host over the same Application modules used by the API.
- Hosts must compose explicit per-module registration methods instead of a root `AddApplication` method.
- Provider-specific DTOs, HTTP details, SQL details and SDK concepts must not leak into Application or Domain contracts.

## Current Design Decisions

- Runtime: .NET 10 LTS.
- API surface: `/api/v1/...`.
- Architecture: Clean Architecture with simple domain records and module-owned application workflow behavior.
- CQRS: CQRS-lite by feature/action folder, not separate read/write systems.
- Pipeline: lightweight internal dispatcher instead of a required MediatR dependency.
- Validation: FluentValidation runs in the dispatcher pipeline; feature normalizers build handler-ready value objects when needed.
- Persistence: raw Npgsql for explicit PostgreSQL and pgvector behavior.
- Ingestion: PostgreSQL-backed indexing jobs processed by `GenAIPlatform.Worker`.
- Auth: foreground `IUserContext` for API/CLI callers, `IBackgroundUserContext`
  for Worker/system jobs, and demo header auth only for local/sample use.
- Providers: deterministic mock providers by default; OpenAI-compatible adapters are replaceable infrastructure adapters.
- Evaluations: API and CLI entry points share Application module services.
- Agentic chat: model proposes tool calls; backend policy decides whether they run.

## GenAI Safety Rules

- The LLM is not a security boundary.
- Apply tenant, owner, access-level and metadata filters before adding document text to prompts.
- Never rely on a prompt instruction to protect private data.
- Full rendered prompt logging must stay disabled by default.
- Do not log document text, rendered prompts, provider credentials, full connection strings, API keys or embedding vectors.
- Automated tests must not call real model or embedding providers by default.
- Tool execution must be deterministic backend behavior. The model may propose actions; backend validation, policy and approval state decide execution.
- Unknown, forbidden or invalid tool calls must fail closed and be audit-visible.

## Reliability Rules

- Treat Application ports as contracts. Do not rely on one adapter's incidental behavior to make a workflow safe.
- Normalize infrastructure-specific exceptions at the Application port boundary; follow `docs/code-organization.md` Infrastructure Error Boundary.
- For workflows crossing storage, persistence, workers, leases, retries, external providers, auth or cleanup, reason about partial failure states before changing behavior.
- Do not return public success after a required durable side effect failed unless a documented recovery invariant and tests prove the state is safe.
- Worker provider calls must be cancelable or explicitly observed when shutdown, cancellation, stale lease or ownership loss occurs.
- Retry and attempt accounting must reflect whether storage/provider side effects may already have happened.
- Prefer targeted fault-injection tests for commit failure, cleanup failure, provider timeout/retry/malformed response, stale lease and duplicate-worker races.
- PostgreSQL/Testcontainers coverage should run for persistence, schema, retrieval, worker lease and migration changes. If Docker-backed tests are skipped locally, call out the residual risk.

## Code Organization

- Follow `docs/code-organization.md`.
- Keep production classes under 200 physical lines unless a local exception is clearly easier to defend than a split.
- Keep one public/internal type per file: class, record, struct, enum or interface.
- Use feature/action folders for Application use cases: `Command.cs`, `Query.cs`, `Handler.cs`, `Validator.cs`, `Response.cs`.
- Keep handlers as orchestration. Put parsing, rendering, policy, persistence detail and provider detail behind named collaborators.
- Keep endpoint handlers transport-focused: bind input, dispatch Application request, map response.
- Put business validation in validators or named policies, not endpoint lambdas.
- Centralize public error-code and HTTP-status mapping.
- Prefer typed options and constants over scattered configuration strings.
- Add abstractions only when they protect a real boundary or remove meaningful duplication.
- Use explicit names that describe the workflow concept, not vague helpers.

## Documentation Rules

- Keep public docs synchronized with behavior changes.
- If a change affects setup, security posture, provider behavior, request/response shape, persistence, evaluation semantics or tool policy, update the matching file under `docs/` and the README when needed.
- Preserve honest framing: this is a starter kit/reference implementation, not a production deployment.
- Document intentional trade-offs instead of hiding them.
- Keep release notes factual: implemented features, defaults, known non-goals and verification scope.

## Verification

Use the smallest reliable check first, then broaden when the changed surface warrants it.

Common commands:

```powershell
dotnet restore GenAIPlatform.slnx
dotnet build GenAIPlatform.slnx
dotnet test GenAIPlatform.slnx
dotnet format GenAIPlatform.slnx --verify-no-changes --verbosity minimal
powershell -ExecutionPolicy Bypass -File scripts\package-vulnerability-gate.ps1
powershell -ExecutionPolicy Bypass -File scripts\code-organization-gate.ps1
```

For persistence-sensitive changes:

```powershell
$env:GENAI_REQUIRE_DOCKER_TESTS = "true"
dotnet test tests\GenAIPlatform.IntegrationTests\GenAIPlatform.IntegrationTests.csproj
```

## When Unsure

Prefer the existing architecture and documented trade-offs. Ask only when a decision changes architecture, versioning, security posture or public behavior. For local implementation details, make the conservative choice that keeps the starter kit understandable.
