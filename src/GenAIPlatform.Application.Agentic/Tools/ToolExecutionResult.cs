using GenAIPlatform.Domain.Agentic;
using System.Text.Json;

namespace GenAIPlatform.Application.Agentic.Tools;

public sealed record ToolExecutionResult(
    ToolExecutionStatus Status,
    JsonElement Output,
    string? ErrorCode = null,
    string? ErrorMessage = null);
