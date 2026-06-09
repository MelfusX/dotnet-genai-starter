# Versioning

This project versions more than releases. GenAI behavior depends on code, prompts, models, retrieval settings, embeddings, evaluations and pricing.

## Project Releases

- Use SemVer.
- Stay in `0.x` until extension points and public contracts stabilize.
- Treat `v0.1.0` as the first public reference release.
- Reserve `v1.0.0` for a future stable starter kit.

## API

- Use `/api/v1/...` from the start.
- Avoid breaking documented `v1` response contracts once examples depend on them.

## Database

- Keep schema changes in source control.
- Document ingestion and retrieval currently use explicit raw SQL/init scripts and small Npgsql adapters while the persistence surface is still stabilizing.
- If EF Core is introduced later for broader persistence, use migrations and name them after the use case or schema change.

## Prompts

- Unique identity: `(templateName, version)`.
- Prompt versions are immutable after activation.
- Store content hash.
- Log template name, version and content hash for every AI request.

## Documents, Chunks and Embeddings

- Documents have versions.
- Chunks are tied to document version, chunking profile version, position and text hash.
- Embeddings store provider, model, dimensions, chunking profile version and created timestamp.
- Retrieval stores both relational `real[]` values and pgvector values so historical metadata remains inspectable while search can use pgvector operators.
- Default RAG retrieval uses the current `documents.version` and excludes older chunk versions unless a future explicit historical retrieval mode is added.
- RAG retrieval filters by embedding provider and model. After an embedding provider/model change, re-index documents before expecting those new embeddings to retrieve older content.
- Re-indexing creates new records instead of silently changing old retrieval history.

## Evaluations

Evaluation runs record:

- dataset version;
- runner version;
- model and settings;
- prompt version;
- retrieval configuration.

## Pricing

Pricing records include effective dates so historical cost calculations remain reproducible.

## Tool Calls

- tool name;
- tool schema version;
- tool policy version.

These fields keep proposed, approved, rejected and executed tool calls
reproducible after a tool schema or backend policy changes.
