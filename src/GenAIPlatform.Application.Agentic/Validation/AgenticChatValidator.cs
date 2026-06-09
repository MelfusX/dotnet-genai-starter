using GenAIPlatform.Application.Generation.ModelGateway;
using GenAIPlatform.Application.Core.ModelClients;
using GenAIPlatform.Application.Agentic.Chat;
using Microsoft.Extensions.Options;

namespace GenAIPlatform.Application.Agentic.Validation;

internal sealed class AgenticChatValidator(IOptions<ModelGatewayOptions> options)
    : ModelChatCommandValidator<AgenticChatCommand>(options);
