using GenAIPlatform.Application.Core.Security;

namespace GenAIPlatform.Application.Agentic.Tools;

public sealed class DemoAgentToolRegistry(IUserContext userContext) : IAgentToolRegistry
{
    public IReadOnlyList<IAgentTool> GetAvailableTools()
    {
        return
        [
            new GetCurrentUserProfileTool(userContext),
            new CreateSupportTicketTool(),
            new DraftEmailTool()
        ];
    }
}
