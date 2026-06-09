using GenAIPlatform.Application.Agentic.Validation;
using GenAIPlatform.Domain.Agentic;
using System.Text.Json;
using GenAIPlatform.Application.Core.ModelClients;

namespace GenAIPlatform.Application.Agentic.Tools;

public interface IAgentTool
{
    AiToolDefinition Definition { get; }

    ToolPolicyMetadata Policy { get; }

    ToolValidationResult Validate(JsonElement arguments);

    Task<ToolExecutionResult> ExecuteAsync(
        JsonElement sanitizedArguments,
        CancellationToken cancellationToken);
}
