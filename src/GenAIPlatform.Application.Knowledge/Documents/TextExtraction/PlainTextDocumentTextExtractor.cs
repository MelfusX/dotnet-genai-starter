using System.Text;
using GenAIPlatform.Domain.Documents;

namespace GenAIPlatform.Application.Knowledge.Documents;

public sealed class PlainTextDocumentTextExtractor : ITextExtractor
{
    private static readonly Encoding StrictUtf8 = new UTF8Encoding(
        encoderShouldEmitUTF8Identifier: false,
        throwOnInvalidBytes: true);

    public async Task<string> ExtractAsync(
        Document document,
        Stream content,
        CancellationToken cancellationToken)
    {
        if (document.SourceExtension is not ".txt" and not ".md")
        {
            throw new DocumentValidationException(
                $"Document extension '{document.SourceExtension}' is not supported for text extraction.");
        }

        using var reader = new StreamReader(
            content,
            StrictUtf8,
            detectEncodingFromByteOrderMarks: true,
            bufferSize: 4096,
            leaveOpen: true);

        try
        {
            return await reader.ReadToEndAsync(cancellationToken);
        }
        catch (DecoderFallbackException exception)
        {
            throw new DocumentValidationException(
                "Document text must be valid UTF-8.",
                exception);
        }
    }
}
