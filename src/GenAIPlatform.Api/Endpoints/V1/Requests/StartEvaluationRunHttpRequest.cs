namespace GenAIPlatform.Api.Endpoints.V1.Requests;

internal sealed record StartEvaluationRunHttpRequest(
    string? DatasetVersion,
    string? Model,
    double? Temperature,
    int? MaxOutputTokens,
    int? TopK,
    double? MinSimilarityScore,
    string? CorrelationId);
