# Changelog

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
