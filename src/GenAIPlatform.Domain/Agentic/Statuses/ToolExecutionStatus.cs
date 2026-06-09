using System.Text.Json.Serialization;

namespace GenAIPlatform.Domain.Agentic;

[JsonConverter(typeof(JsonStringEnumConverter<ToolExecutionStatus>))]
public enum ToolExecutionStatus
{
    NotExecuted,
    Rejected,
    ValidationFailed,
    ApprovalRequired,
    Failed,
    Succeeded
}
