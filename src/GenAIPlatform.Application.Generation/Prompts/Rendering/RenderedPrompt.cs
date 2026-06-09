using GenAIPlatform.Domain.Prompts;

namespace GenAIPlatform.Application.Generation.Prompts.Rendering;

public sealed record RenderedPrompt(
    string SystemMessage,
    string UserMessage,
    PromptMetadata Metadata);
