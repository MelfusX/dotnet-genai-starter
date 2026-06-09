using Microsoft.Extensions.Options;

namespace GenAIPlatform.Application.Knowledge.Embeddings;

/// <summary>
/// Validator for <see cref="EmbeddingOptions"/>. The Validate implementation is generated
/// at compile time from the data annotations on the options class via
/// <see cref="OptionsValidatorAttribute"/>.
/// </summary>
[OptionsValidator]
internal sealed partial class EmbeddingOptionsValidator
    : IValidateOptions<EmbeddingOptions>;
