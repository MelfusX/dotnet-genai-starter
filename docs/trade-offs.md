# Trade-offs

This document records intentional choices and their costs.

## pgvector vs Azure AI Search

pgvector is simple, local and cost-effective. Azure AI Search is more enterprise-ready and feature-rich. The starter kit begins with pgvector and allows an Azure AI Search adapter later.

## pgvector Dimension-Specific Indexes

The retrieval schema stores embeddings in a dimensionless pgvector column so mock and real embedding providers can coexist during the starter-kit phase. HNSW indexes require fixed dimensions, so the init scripts create partial indexes for the default 16-dimension mock embeddings and common 1536-dimension embeddings. Other dimensions still work through exact search until a deployment chooses and indexes its production embedding size.

Retrieval still filters by embedding provider and model before ranking. Dimension-only compatibility is not enough because different embedding models can produce vectors in incompatible spaces even when their dimensions match.

## Simple Prompt Templates vs Full Prompt Management

Simple templates are enough for the starter-kit scope. Full prompt management may need UI, approvals, diffs, evaluation gates and rollout strategy.

## Mock Model vs Real Model In Tests

Real model calls are expensive and nondeterministic. Automated tests use mock clients by default.

## Full Prompt Logging vs Privacy

Full prompt logs help debugging but may leak sensitive data. Default logging is metadata-only.

## Simple Access Control vs Enterprise RBAC

The current implementation uses private and tenant-public document access metadata only. Shared user/group grants are deferred until durable grant metadata and retrieval filters are implemented; the architecture should not block later RBAC or Entra ID integration.

## Domain Records with Application-Owned Behavior

Domain types are intentionally simple records and enums in the starter-kit scope, but domain concepts live in the Domain layer so they can be reused across Application workflows without creating Application-to-Application coupling. Workflow behavior, validation policy and partial-failure handling stay in Application services so the public sample remains easy to inspect without DDD ceremony.

## Simple Evals vs LLM-as-Judge

Rule-based evals are deterministic and easy. LLM-as-judge can be useful later, but adds cost and nondeterminism.

## Starter Kit vs Framework

A starter kit is easier to build and understand. A framework requires stable APIs, compatibility guarantees and long-term support.

## Internal Dispatcher vs MediatR

A lightweight internal dispatcher keeps the starter kit dependency-light. MediatR v12 can be familiar for many .NET developers, but newer MediatR versions may introduce licensing considerations. This project uses an internal dispatcher/pipeline and can document MediatR as an optional alternative later.

## FluentValidation vs Custom Validators

FluentValidation is used for request-shape validation because it is familiar to many .NET teams, has no MediatR dependency and keeps rule composition separate from handler orchestration. Handlers that need normalized value objects use a neighboring `Normalizer.cs` instead of asking validators to both reject invalid input and build workflow state.

The MediatR decision remains separate. This project still uses its internal dispatcher and pipeline behaviors; FluentValidation provides the rule engine only.

## .NET 10 LTS vs Older Targets

.NET 10 LTS is the preferred baseline for a new project started in 2026. Older .NET versions may be familiar to more teams, but they have shorter remaining support windows.

## `v0.1.0` vs `v1.0.0`

`v0.1.0` communicates that the project is useful but evolving. `v1.0.0` should wait until contracts, docs and extension points are stable.

## Raw String Identifiers vs Strongly-Typed Value Objects

Identifiers like `TenantId`, `UserId`, `DocumentId` are passed as `string` and `Guid` throughout the codebase rather than as strongly-typed value objects (e.g. `readonly record struct TenantId`). Value objects offer compile-time safety against argument-mix-ups and centralized validation, but introduce friction with `System.Text.Json`, `Npgsql` parameter binding, and `IOptions<T>` binding at the starter-kit scope. The current implementation accepts the small risk of string mix-ups in exchange for transport simplicity. A future scope that grows multi-context handler signatures (tenant + user + correlation + ...) may revisit this.
