using GenAIPlatform.Application.Knowledge.Documents;

namespace GenAIPlatform.Application.Knowledge.Documents.ProcessIndexingJobs.Failure;

internal sealed class IndexingAttemptState
{
    public bool AttemptConsumed { get; private set; }

    public void MarkAttemptConsumed()
    {
        AttemptConsumed = true;
    }
}
