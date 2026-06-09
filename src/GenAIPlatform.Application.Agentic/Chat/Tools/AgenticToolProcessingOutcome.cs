using GenAIPlatform.Domain.Agentic;

namespace GenAIPlatform.Application.Agentic.Chat;

internal sealed record AgenticToolProcessingOutcome(
    AgenticChatStatus Status,
    string Answer,
    int Step)
{
    public bool IsTerminal { get; init; } = true;

    public static AgenticToolProcessingOutcome Continue(int step)
    {
        return new AgenticToolProcessingOutcome(
            AgenticChatStatus.Succeeded,
            string.Empty,
            step)
        {
            IsTerminal = false
        };
    }
}
