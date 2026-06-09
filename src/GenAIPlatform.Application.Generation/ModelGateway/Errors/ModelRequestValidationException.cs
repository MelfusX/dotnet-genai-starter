using GenAIPlatform.Application.Core.Exceptions;

namespace GenAIPlatform.Application.Generation.ModelGateway;

public sealed class ModelRequestValidationException(string message) : ValidationException(message);
