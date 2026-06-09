using GenAIPlatform.Application.Knowledge.Documents;

namespace GenAIPlatform.Application.Knowledge.Documents.ProcessIndexingJobs.Failure;

internal enum IndexingFailureRecord
{
    None,
    Retried,
    Failed
}
