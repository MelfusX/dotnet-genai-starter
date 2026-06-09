namespace GenAIPlatform.Application.Core.ModelClients;

/// <summary>
/// Carries provider-reported token usage for a model response.
/// </summary>
/// <remarks>
/// Implementations must pass through provider-reported values without deriving costs or fabricating missing token counts.
/// </remarks>
/// <param name="InputTokens">The provider-reported input token count when available.</param>
/// <param name="OutputTokens">The provider-reported output token count when available.</param>
/// <param name="TotalTokens">The provider-reported total token count when available.</param>
public sealed record AiModelUsage(
    int? InputTokens,
    int? OutputTokens,
    int? TotalTokens);
