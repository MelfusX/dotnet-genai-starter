using System.Text.Json;

namespace GenAIPlatform.Api.Endpoints.V1.Requests;

internal sealed record RagChatHttpRequest(
    string? Message,
    string? Model,
    double? Temperature,
    int? MaxOutputTokens,
    int? TopK,
    double? MinSimilarityScore,
    JsonElement DocumentIds,
    string? CorrelationId);
