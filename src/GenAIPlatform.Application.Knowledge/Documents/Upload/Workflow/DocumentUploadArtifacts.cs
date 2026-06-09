using GenAIPlatform.Domain.Documents;

namespace GenAIPlatform.Application.Knowledge.Documents;

internal sealed record DocumentUploadArtifacts(
    Document Document,
    IndexingJob IndexingJob);
