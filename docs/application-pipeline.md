# Application Pipeline

The application layer uses a lightweight internal dispatcher. The dispatcher can run application pipeline behaviors when they are registered, without forcing MediatR as a core dependency.

## Intent

Use explicit use cases:

```text
Command/Query -> Handler -> Result
```

The v0.1 dispatcher supports cross-cutting behavior around handlers:

```text
optional registered behaviors
-> handler
```

No pipeline behaviors are registered by default in v0.1. Validation, correlation IDs, request logging, audit writes and budget checks currently live in the relevant handlers, workflows and application services. A future iteration can move repeated behavior into `IPipelineBehavior<,>` implementations when there is enough shared shape to justify it.

## Decision

- Implement a small internal dispatcher/pipeline in the application layer.
- Keep the default registration behavior-free until a real shared behavior is implemented and tested.
- Do not add MediatR as a required core dependency.
- MediatR v12 can be documented later as an optional adapter for teams that prefer it.

## Boundaries

- Pipeline behaviors should be application concerns, not HTTP middleware replacements.
- HTTP-only concerns stay in `GenAIPlatform.Api`.
- Persistence transactions can be opened by an application pipeline behavior but implemented by Infrastructure.
- AI request logging should be triggered consistently through application services and/or pipeline behaviors.

## Use Case Layout

Pipeline use cases should live under feature/action folders. The folder carries the domain context, so file names should stay simple:

```text
Chat/
  Direct/
    Command.cs
    Handler.cs
    Validator.cs
    Response.cs
```

Use `Query.cs` instead of `Command.cs` for read-only use cases. If a type name such as `DirectChatCommand` is needed to understand the behavior, the folder structure or use-case name is probably too vague.

For non-trivial commands and queries, place `Validator.cs` next to the request and handler. API validation should stay limited to HTTP transport shape; business input validation belongs in the application layer.

Handlers should remain orchestration code. When a handler grows large or mixes validation, persistence, provider calls, parsing, prompt rendering and response shaping, split those responsibilities behind named application services, policies, workflow coordinators or infrastructure adapters.

## Non-goals

- No event sourcing in the starter-kit scope.
- No separate read/write databases in the starter-kit scope.
- No dependency on a commercial mediator package.
