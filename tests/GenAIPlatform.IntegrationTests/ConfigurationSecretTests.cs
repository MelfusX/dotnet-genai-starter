using System.Runtime.CompilerServices;

namespace GenAIPlatform.IntegrationTests;

public sealed class ConfigurationSecretTests
{
    [Theory]
    [InlineData("src/GenAIPlatform.Api/appsettings.json")]
    [InlineData("src/GenAIPlatform.Worker/appsettings.json")]
    [InlineData("src/GenAIPlatform.Evaluations/appsettings.json")]
    public async Task RuntimeAppSettings_DoNotContainPostgresPasswords(string relativePath)
    {
        var content = await File.ReadAllTextAsync(
            Path.Combine(FindRepositoryRoot(), relativePath));

        Assert.DoesNotContain("genai_dev_password", content, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("Password=", content, StringComparison.OrdinalIgnoreCase);
    }

    private static string FindRepositoryRoot(
        [CallerFilePath] string sourceFilePath = "")
    {
        foreach (var startPath in new[] { sourceFilePath, AppContext.BaseDirectory, Directory.GetCurrentDirectory() })
        {
            var directory = File.Exists(startPath)
                ? new FileInfo(startPath).Directory
                : new DirectoryInfo(startPath);
            while (directory is not null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "GenAIPlatform.slnx")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }
        }

        throw new InvalidOperationException("Could not find repository root.");
    }
}
