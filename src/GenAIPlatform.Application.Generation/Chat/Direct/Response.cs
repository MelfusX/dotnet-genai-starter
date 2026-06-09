using GenAIPlatform.Domain.Prompts;
using GenAIPlatform.Application.Generation.ModelGateway;
using GenAIPlatform.Application.Core.ModelClients;
using GenAIPlatform.Application.Generation.Prompts;

namespace GenAIPlatform.Application.Generation.Chat;

public sealed record DirectChatResponse(
    string Message,
    string Model,
    string Provider,
    AiModelUsage? Usage,
    PromptMetadata Prompt,
    string CorrelationId);
