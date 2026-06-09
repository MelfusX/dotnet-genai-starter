using GenAIPlatform.Domain.Prompts;
using GenAIPlatform.Application.Generation.ModelGateway;
using GenAIPlatform.Application.Core.ModelClients;
using GenAIPlatform.Application.Generation.Prompts;

namespace GenAIPlatform.Application.Agentic.Chat;

public sealed record AgenticPromptMessages(
    IReadOnlyList<AiChatMessage> Messages,
    PromptMetadata Prompt);
