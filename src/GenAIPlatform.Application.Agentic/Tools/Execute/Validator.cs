using FluentValidation;

namespace GenAIPlatform.Application.Agentic.Tools.Execute;

internal sealed class ExecuteToolCommandValidator : AbstractValidator<ExecuteToolCommand>
{
    public ExecuteToolCommandValidator()
    {
        RuleFor(static command => command.ToolName)
            .NotEmpty()
            .MaximumLength(128);
    }
}
