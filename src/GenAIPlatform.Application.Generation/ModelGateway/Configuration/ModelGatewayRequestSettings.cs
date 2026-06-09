namespace GenAIPlatform.Application.Generation.ModelGateway;

public sealed record ModelGatewayRequestSettings(
    string CorrelationId,
    string Model,
    double Temperature,
    int MaxOutputTokens);
