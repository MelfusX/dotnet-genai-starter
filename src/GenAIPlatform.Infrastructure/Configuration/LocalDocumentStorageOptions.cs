namespace GenAIPlatform.Infrastructure.Configuration;

public sealed class LocalDocumentStorageOptions
{
    public const string SectionName = "GenAIPlatform:DocumentStorage";

    public string RootPath { get; init; } = "storage/documents";
}
