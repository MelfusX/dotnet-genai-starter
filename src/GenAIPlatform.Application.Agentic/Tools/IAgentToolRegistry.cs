namespace GenAIPlatform.Application.Agentic.Tools;

public interface IAgentToolRegistry
{
    IReadOnlyList<IAgentTool> GetAvailableTools();
}
