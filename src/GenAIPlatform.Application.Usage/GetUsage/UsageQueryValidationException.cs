using GenAIPlatform.Application.Core.Exceptions;

namespace GenAIPlatform.Application.Usage.GetUsage;

public sealed class UsageQueryValidationException(string message)
    : ValidationException(message);
