namespace GenAIPlatform.Application.Core.Embeddings;

/// <summary>
/// Carries a normalized embedding response across the application port boundary.
/// </summary>
/// <remarks>
/// Implementations must return vectors in provider order without truncation and must identify the adapter in <see cref="Provider" /> for audit and retrieval compatibility checks.
/// </remarks>
/// <param name="Vector">The provider-returned embedding vector.</param>
/// <param name="Model">The provider embedding model name that produced the vector.</param>
/// <param name="Provider">The stable adapter identifier that produced the vector.</param>
/// <param name="InputTokens">The provider-reported input token count when available.</param>
/// <param name="CorrelationId">The optional application correlation identifier associated with the request.</param>
public sealed record EmbeddingResponse(
    IReadOnlyList<float> Vector,
    string Model,
    string Provider,
    int? InputTokens,
    string? CorrelationId);
