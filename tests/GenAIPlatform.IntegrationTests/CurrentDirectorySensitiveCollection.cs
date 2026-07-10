namespace GenAIPlatform.IntegrationTests;

// Environment.CurrentDirectory is process-global. Tests in this collection must not overlap
// WebApplicationFactory startup or any other content-root discovery in the test process.
[CollectionDefinition(DisableParallelization = true)]
public sealed class CurrentDirectorySensitiveCollection
{
}
