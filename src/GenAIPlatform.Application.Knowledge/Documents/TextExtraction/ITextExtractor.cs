using GenAIPlatform.Domain.Documents;

namespace GenAIPlatform.Application.Knowledge.Documents;

public interface ITextExtractor
{
    Task<string> ExtractAsync(
        Document document,
        Stream content,
        CancellationToken cancellationToken);
}
