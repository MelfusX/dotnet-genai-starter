namespace GenAIPlatform.Api.Endpoints.V1.Requests;

internal sealed record AgenticChatHttpRequest(
    string? Message,
    string? Model,
    double? Temperature,
    int? MaxOutputTokens,
    string? CorrelationId,
    bool? ApproveRiskyTools);
