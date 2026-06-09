using System.Text.RegularExpressions;
using GenAIPlatform.Application.Core.ModelClients;
using Microsoft.Extensions.Options;

namespace GenAIPlatform.Application.Generation.ModelGateway;

public sealed partial class ModelGatewayRequestPolicy(IOptions<ModelGatewayOptions> options)
{
    private const int DefaultMaxInputMessageCharacters = 8000;
    private const int MaxRetrySafeCorrelationIdLength = 128;

    public int GetMaxInputMessageCharacters()
    {
        var currentOptions = options.Value;
        return currentOptions.MaxInputMessageCharacters > 0
            ? currentOptions.MaxInputMessageCharacters
            : DefaultMaxInputMessageCharacters;
    }

    public void ValidateInputMessages(IReadOnlyList<AiChatMessage> messages)
    {
        ArgumentNullException.ThrowIfNull(messages);
        if (messages.Count == 0)
        {
            throw new ModelRequestValidationException(
                "Model input messages must not be empty.");
        }

        var maxInputMessageCharacters = GetMaxInputMessageCharacters();
        var totalCharacters = 0;

        foreach (var message in messages)
        {
            if (string.IsNullOrWhiteSpace(message.Content))
            {
                throw new ModelRequestValidationException(
                    "Model input messages must not be empty.");
            }

            totalCharacters += message.Content.Length;
            if (totalCharacters > maxInputMessageCharacters)
            {
                throw new ModelRequestValidationException(
                    $"Combined model input messages must be {maxInputMessageCharacters} characters or fewer.");
            }
        }
    }

    public ModelGatewayRequestSettings Resolve(
        string? requestedModel,
        double? requestedTemperature,
        int? requestedMaxOutputTokens,
        string? requestedCorrelationId)
    {
        var currentOptions = options.Value;
        var model = ResolveModel(currentOptions, requestedModel);
        var temperature = ResolveTemperature(currentOptions, requestedTemperature);
        var maxOutputTokens = ResolveMaxOutputTokens(currentOptions, requestedMaxOutputTokens);
        var correlationId = ResolveCorrelationId(currentOptions, requestedCorrelationId);

        return new ModelGatewayRequestSettings(
            correlationId,
            model,
            temperature,
            maxOutputTokens);
    }

    private static string ResolveModel(ModelGatewayOptions options, string? requestedModel)
    {
        if (string.IsNullOrWhiteSpace(options.DefaultModel))
        {
            throw new InvalidOperationException("Default model gateway model is not configured.");
        }

        var routeModels = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["default"] = options.DefaultModel,
            ["strong"] = options.StrongModel,
            ["cheap"] = options.CheapModel,
            ["evaluation"] = options.EvaluationModel
        };

        var requested = requestedModel?.Trim();
        var resolvedModel = string.IsNullOrWhiteSpace(requested)
            ? options.DefaultModel
            : routeModels.GetValueOrDefault(requested, requested);

        var allowedModels = BuildAllowedModelSet(options, routeModels.Values);
        if (!allowedModels.Contains(resolvedModel))
        {
            throw new ModelRequestValidationException("Requested model is not allowed.");
        }

        return resolvedModel;
    }

    private static double ResolveTemperature(ModelGatewayOptions options, double? requestedTemperature)
    {
        var temperature = requestedTemperature ?? options.DefaultTemperature;
        if (double.IsNaN(temperature) ||
            double.IsInfinity(temperature) ||
            temperature < options.MinTemperature ||
            temperature > options.MaxTemperature)
        {
            throw new ModelRequestValidationException(
                $"Temperature must be between {options.MinTemperature} and {options.MaxTemperature}.");
        }

        return temperature;
    }

    private static int ResolveMaxOutputTokens(ModelGatewayOptions options, int? requestedMaxOutputTokens)
    {
        var maxOutputTokens = requestedMaxOutputTokens ?? options.DefaultMaxOutputTokens;
        if (maxOutputTokens < 1 || maxOutputTokens > options.MaxOutputTokensLimit)
        {
            throw new ModelRequestValidationException(
                $"Max output tokens must be between 1 and {options.MaxOutputTokensLimit}.");
        }

        return maxOutputTokens;
    }

    private static string ResolveCorrelationId(ModelGatewayOptions options, string? requestedCorrelationId)
    {
        if (string.IsNullOrWhiteSpace(requestedCorrelationId))
        {
            return Guid.NewGuid().ToString("n");
        }

        var maxLength = options.MaxCorrelationIdLength is > 0 and <= MaxRetrySafeCorrelationIdLength
            ? options.MaxCorrelationIdLength
            : MaxRetrySafeCorrelationIdLength;

        var correlationId = requestedCorrelationId.Trim();
        if (correlationId.Length > maxLength ||
            !CorrelationIdPattern().IsMatch(correlationId))
        {
            throw new ModelRequestValidationException(
                $"Correlation id must be {maxLength} characters or fewer and contain only letters, digits, '.', '_', ':' or '-'.");
        }

        return correlationId;
    }

    private static HashSet<string> BuildAllowedModelSet(
        ModelGatewayOptions options,
        IEnumerable<string> routeModels)
    {
        var allowedModels = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var model in routeModels.Concat(options.AllowedModels))
        {
            if (!string.IsNullOrWhiteSpace(model))
            {
                allowedModels.Add(model.Trim());
            }
        }

        return allowedModels;
    }

    [GeneratedRegex("^[A-Za-z0-9][A-Za-z0-9._:-]*$", RegexOptions.CultureInvariant)]
    private static partial Regex CorrelationIdPattern();
}
