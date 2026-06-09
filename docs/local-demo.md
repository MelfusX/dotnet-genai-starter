# Local Demo Walkthrough

This walkthrough is the recommended path for reviewing the starter kit locally.

## What To Run First

- Start PostgreSQL with Docker Compose.
- Run the API and Worker hosts.
- Upload `samples/documents/demo-notes.md`.
- Let the worker index the sample document.
- Ask a RAG question and inspect citations.
- Query usage to see sanitized request metadata and cost estimates.
- Run the sample evaluation dataset through API or CLI.
- Run bounded agentic chat and inspect policy-controlled tool behavior.

## Why This Path Uses Mocks

The default local demo runs with deterministic mock model and embedding providers. That keeps the full flow repeatable without real LLM credentials, network access or provider cost.

OpenAI-compatible chat and embedding adapters are included behind Application ports. Enable them through local configuration when you want to test real provider behavior.

## Summary

.NET-native GenAI Platform Starter Kit demonstrates the parts many GenAI demos skip: secure RAG, model gateway abstraction, prompt versioning, AI request logging, cost tracking, evaluations and backend-controlled tool execution.
