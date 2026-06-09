# GenAI Platform Demo Notes

This safe sample document is intended for local RAG smoke tests.

The starter kit demonstrates direct chat, document upload, pgvector-backed retrieval, usage tracking, evaluations and bounded agentic chat. It uses mock model and embedding providers by default so the public quickstart does not need real provider credentials.

Key release guardrails:

- Retrieval applies tenant, access-level, document, current-version and embedding compatibility filters before document text is added to a prompt.
- Full rendered prompt logging is disabled by default.
- Safe tool execution is controlled by backend policy, not by the model.
