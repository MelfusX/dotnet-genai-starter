using System.Text.RegularExpressions;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace GenAIPlatform.Application.Generation.ModelGateway;

/// <summary>
/// Base FluentValidation rules shared by chat-style commands targeting the model gateway
/// (message length, sampling bounds, correlation id format). Concrete validators inherit
/// this to avoid duplicating the identical rule set across feature folders.
/// </summary>
public abstract partial class ModelChatCommandValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : IModelChatCommand
{
    private const int DefaultMaxInputMessageCharacters = 8000;
    private const int MaxRetrySafeCorrelationIdLength = 128;

    protected ModelChatCommandValidator(IOptions<ModelGatewayOptions> options)
    {
        var settings = options.Value;
        var maxInputMessageCharacters = settings.MaxInputMessageCharacters > 0
            ? settings.MaxInputMessageCharacters
            : DefaultMaxInputMessageCharacters;
        var maxCorrelationIdLength = settings.MaxCorrelationIdLength is > 0 and <= MaxRetrySafeCorrelationIdLength
            ? settings.MaxCorrelationIdLength
            : MaxRetrySafeCorrelationIdLength;

        RuleFor(request => request.Message)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Message must not be empty.")
            .Must(static message => !string.IsNullOrWhiteSpace(message))
                .WithMessage("Message must not be empty.")
            .Must(message => message is null || message.Trim().Length <= maxInputMessageCharacters)
                .WithMessage($"Message must be {maxInputMessageCharacters} characters or fewer.");

        RuleFor(request => request.Temperature)
            .Must(temperature => temperature is null ||
                (!double.IsNaN(temperature.Value) &&
                 !double.IsInfinity(temperature.Value) &&
                 temperature.Value >= settings.MinTemperature &&
                 temperature.Value <= settings.MaxTemperature))
            .WithMessage($"Temperature must be between {settings.MinTemperature} and {settings.MaxTemperature}.");

        RuleFor(request => request.MaxOutputTokens)
            .Must(maxOutputTokens => maxOutputTokens is null ||
                maxOutputTokens.Value is >= 1 &&
                maxOutputTokens.Value <= settings.MaxOutputTokensLimit)
            .WithMessage($"Max output tokens must be between 1 and {settings.MaxOutputTokensLimit}.");

        RuleFor(request => request.CorrelationId)
            .Must(correlationId => HasValidCorrelationId(correlationId, maxCorrelationIdLength))
            .WithMessage($"Correlation id must be {maxCorrelationIdLength} characters or fewer and contain only letters, digits, '.', '_', ':' or '-'.");
    }

    private static bool HasValidCorrelationId(string? requestedCorrelationId, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(requestedCorrelationId))
        {
            return true;
        }

        var correlationId = requestedCorrelationId.Trim();
        return correlationId.Length <= maxLength &&
               CorrelationIdPattern().IsMatch(correlationId);
    }

    [GeneratedRegex("^[A-Za-z0-9][A-Za-z0-9._:-]*$", RegexOptions.CultureInvariant)]
    private static partial Regex CorrelationIdPattern();
}
