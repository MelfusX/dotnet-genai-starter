using GenAIPlatform.Application.Core.Security;

namespace GenAIPlatform.Evaluations;

internal sealed class EvaluationCliUserContext : IUserContext
{
    public bool IsAuthenticated => true;

    public string UserId => "evaluation-cli";

    public string TenantId => "local";

    public IReadOnlyCollection<string> Roles { get; } = ["developer"];

    public IReadOnlyCollection<string> Groups { get; } = ["demo"];
}
