using Microsoft.Extensions.Options;

namespace GenAIPlatform.Infrastructure.Observability.Logging;

/// <summary>
/// Validator for <see cref="AiRequestLoggingOptions"/>. The Validate implementation is generated
/// at compile time from the data annotations on the options class via
/// <see cref="OptionsValidatorAttribute"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class AiRequestLoggingOptionsValidator
    : IValidateOptions<AiRequestLoggingOptions>;
