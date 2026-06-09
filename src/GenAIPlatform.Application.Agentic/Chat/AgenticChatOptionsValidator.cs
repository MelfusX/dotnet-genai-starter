using Microsoft.Extensions.Options;

namespace GenAIPlatform.Application.Agentic;

/// <summary>
/// Validator for <see cref="AgenticChatOptions"/>. The Validate implementation is generated
/// at compile time from the data annotations on the options class via
/// <see cref="OptionsValidatorAttribute"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class AgenticChatOptionsValidator
    : IValidateOptions<AgenticChatOptions>;
