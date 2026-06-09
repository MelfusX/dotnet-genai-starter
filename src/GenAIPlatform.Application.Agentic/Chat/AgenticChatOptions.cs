using System.ComponentModel.DataAnnotations;
using GenAIPlatform.Application.Core.Configuration;

namespace GenAIPlatform.Application.Agentic;

public sealed class AgenticChatOptions
{
    public const string SectionName = "GenAIPlatform:AgenticChat";

    [Range(1, 16)]
    public int MaxSteps { get; init; } = 4;

    [Range(1, 120)]
    public int TimeoutSeconds { get; init; } = 15;

    [Range(1, 32)]
    public int MaxToolCalls { get; init; } = 8;

    [Range(1, int.MaxValue)]
    public int MaxTotalTokens { get; init; } = 4096;

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal MaxEstimatedCost { get; init; } = 0.05m;

    [Range(typeof(decimal), "0", "79228162514264337593543950335")]
    public decimal EstimatedCostPerThousandTokens { get; init; } = 0.001m;

    [RequiredNonBlank]
    public string PolicyVersion { get; init; } = "tool-policy-v1";
}
