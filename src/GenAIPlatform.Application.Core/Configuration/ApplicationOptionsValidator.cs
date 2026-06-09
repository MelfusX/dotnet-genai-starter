using Microsoft.Extensions.Options;

namespace GenAIPlatform.Application.Core.Configuration;

/// <summary>
/// Validator for <see cref="ApplicationOptions"/>. The Validate implementation is generated
/// at compile time from the data annotations on the options class via
/// <see cref="OptionsValidatorAttribute"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class ApplicationOptionsValidator
    : IValidateOptions<ApplicationOptions>;
