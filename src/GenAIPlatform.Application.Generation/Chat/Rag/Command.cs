using GenAIPlatform.Application.Core.Dispatching;

namespace GenAIPlatform.Application.Generation.Chat;

public sealed record RagChatCommand(
    string Message,
    string? Model = null,
    double? Temperature = null,
    int? MaxOutputTokens = null,
    int? TopK = null,
    double? MinSimilarityScore = null,
    IReadOnlyCollection<Guid>? DocumentIds = null,
    string? CorrelationId = null)
    : IRequest<RagChatResponse>;
