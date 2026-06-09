# v0.1.0 Reference Release Notes

`v0.1.0` is the first public reference release for the .NET-native GenAI Platform Starter Kit.

## Included

- Clean Architecture solution targeting .NET 10 LTS.
- `/api/v1` HTTP surface for health, demo user context, direct chat, document upload/status, RAG chat, usage, evaluations and agentic chat.
- OpenAI-compatible model and embedding adapters behind Application ports.
- Deterministic mock model and embedding providers for local demos and automated tests.
- Prompt seed files with version, active status and content hash metadata.
- PostgreSQL-backed document ingestion with DB-backed indexing jobs.
- pgvector retrieval with tenant, access-level, document, current-version and embedding compatibility filters before prompt construction.
- AI request logging, token usage capture, pricing records and usage aggregation.
- Evaluation runner through shared Application services, exposed through API and CLI.
- Bounded agentic chat with backend policy-controlled demo tools and tool audit persistence.
- Docker Compose PostgreSQL with pgvector init scripts.
- Sample document and HTTP requests for the public demo path.

## Security And Privacy Defaults

- Full rendered prompt and response-body logging is disabled by default.
- Automated tests and local quickstart use mock providers by default.
- The model is not treated as a security boundary.
- Tool execution is decided by deterministic backend policy.
- Public samples contain synthetic demo data only.

## Known Non-goals

- This is not a stable public framework API.
- This is not a production SaaS shell.
- Real authentication, enterprise connectors, multi-service deployment and Kubernetes packaging remain future adapter work.
- Prompt management UI, approval workflows and rollout tooling are outside the first public reference release.

## Suggested Tag Notes

```text
v0.1.0 - first public reference release

Includes the .NET 10 Clean Architecture starter kit with mock-provider quickstart, PostgreSQL + pgvector RAG, document ingestion, prompt version metadata, AI request logging, cost/usage tracking, evaluations and bounded agentic chat with backend-controlled safe tools.

Defaults are local-demo safe: mock model/embedding providers, no real provider calls in tests, full prompt logging disabled, and synthetic sample data only.
```
