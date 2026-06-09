using GenAIPlatform.Application.Generation.ModelGateway;
using GenAIPlatform.Application.Core.ModelClients;
using Microsoft.Extensions.Options;

namespace GenAIPlatform.Application.Generation.Chat;

internal sealed class DirectChatValidator(IOptions<ModelGatewayOptions> options)
    : ModelChatCommandValidator<DirectChatCommand>(options);
