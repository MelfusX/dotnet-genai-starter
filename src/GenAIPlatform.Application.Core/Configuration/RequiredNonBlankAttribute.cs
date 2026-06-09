using System.ComponentModel.DataAnnotations;

namespace GenAIPlatform.Application.Core.Configuration;

/// <summary>
/// Stricter alternative to <see cref="RequiredAttribute"/>: rejects null, empty, and
/// whitespace-only strings. <see cref="RequiredAttribute"/> with
/// <c>AllowEmptyStrings = false</c> still treats a single space as valid; configuration
/// options that must carry a meaningful identifier (api version, model name, provider key,
/// policy version, ...) need the whitespace check too.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class RequiredNonBlankAttribute : ValidationAttribute
{
    public RequiredNonBlankAttribute()
        : base("The {0} field must be a non-empty, non-whitespace value.")
    {
    }

    public override bool IsValid(object? value)
    {
        return value is string text && !string.IsNullOrWhiteSpace(text);
    }
}
