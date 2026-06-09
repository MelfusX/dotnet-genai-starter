using FluentValidation.Results;
using GenAIPlatform.Application.Core.Exceptions;

namespace GenAIPlatform.Application.Core.Dispatching;

/// <summary>
/// Thrown by <see cref="RequestValidationBehavior{TRequest,TResponse}" /> when one or more FluentValidation rules fail for the dispatched request.
/// </summary>
/// <remarks>
/// Inherits <see cref="ValidationException" /> so the API exception handler maps it to HTTP 400 without further changes.
/// </remarks>
public sealed class RequestValidationException : ValidationException
{
    public RequestValidationException(IReadOnlyCollection<ValidationFailure> failures)
        : base(FormatMessage(failures))
    {
        Failures = failures;
    }

    public IReadOnlyCollection<ValidationFailure> Failures { get; }

    private static string FormatMessage(IReadOnlyCollection<ValidationFailure> failures) =>
        string.Join("; ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));
}
