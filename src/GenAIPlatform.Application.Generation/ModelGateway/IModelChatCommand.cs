namespace GenAIPlatform.Application.Generation.ModelGateway;

/// <summary>
/// Shape shared by chat-style commands that target the model gateway. Lets a single
/// validator enforce the common request fields (message, sampling and correlation id)
/// without duplicating the rules across each command's feature folder.
/// </summary>
public interface IModelChatCommand
{
    string? Message { get; }

    double? Temperature { get; }

    int? MaxOutputTokens { get; }

    string? CorrelationId { get; }
}
