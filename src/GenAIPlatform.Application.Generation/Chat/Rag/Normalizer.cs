using Microsoft.Extensions.Options;

namespace GenAIPlatform.Application.Generation.Chat;

internal sealed class RagChatNormalizer(IOptions<RagOptions> ragOptions)
{
    public RagChatValidationResult Normalize(RagChatCommand request)
    {
        return new RagChatValidationResult(
            request.Message.Trim(),
            request.TopK ?? ragOptions.Value.DefaultTopK,
            request.MinSimilarityScore ?? ragOptions.Value.DefaultMinSimilarityScore,
            NormalizeDocumentIds(request.DocumentIds));
    }

    private static IReadOnlyCollection<Guid> NormalizeDocumentIds(
        IReadOnlyCollection<Guid>? requestedDocumentIds)
    {
        return requestedDocumentIds is null
            ? []
            : requestedDocumentIds.Distinct().ToArray();
    }
}
