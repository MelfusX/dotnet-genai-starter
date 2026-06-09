namespace GenAIPlatform.Application.Generation.ModelGateway;

public sealed class ModelGatewayOptions
{
    public const string SectionName = "GenAIPlatform:ModelGateway";

    public string Provider { get; init; } = "Mock";

    public string DefaultModel { get; init; } = "mock-chat";

    public string StrongModel { get; init; } = "mock-chat-strong";

    public string CheapModel { get; init; } = "mock-chat-cheap";

    public string EvaluationModel { get; init; } = "mock-chat-evaluation";

    public double DefaultTemperature { get; init; } = 0.2;

    public int DefaultMaxOutputTokens { get; init; } = 512;

    public int MaxInputMessageCharacters { get; init; } = 8000;

    public IReadOnlyCollection<string> AllowedModels { get; init; } = [];

    public double MinTemperature { get; init; } = 0;

    public double MaxTemperature { get; init; } = 1;

    public int MaxOutputTokensLimit { get; init; } = 2048;

    public int MaxCorrelationIdLength { get; init; } = 128;
}
