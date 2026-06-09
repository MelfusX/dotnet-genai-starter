using GenAIPlatform.Application.Evaluations;
using GenAIPlatform.Domain.Observability;
using GenAIPlatform.Application.Knowledge.Embeddings;
using GenAIPlatform.Application.Core.Embeddings;
using GenAIPlatform.Application.Knowledge.Retrieval;

namespace GenAIPlatform.Application.Evaluations.StartRun.Context;

internal sealed record EvaluationRetrievalContext(
    string Message,
    string ContextText,
    IReadOnlyList<RetrievedDocumentChunk> Chunks,
    TimeSpan RetrievalLatency,
    EmbeddingResponse? Embedding,
    IReadOnlyList<RetrievedDocumentReference> RetrievedDocuments);
