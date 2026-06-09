using System.ComponentModel.DataAnnotations;

namespace GenAIPlatform.Infrastructure.Observability.Logging;

public sealed class AiRequestLoggingOptions
{
    public const string SectionName = "GenAIPlatform:Observability:AiRequestLogging";

    [EnumDataType(typeof(AiRequestLoggingFailureMode))]
    public AiRequestLoggingFailureMode FailureMode { get; init; } = AiRequestLoggingFailureMode.FailOpen;
}
