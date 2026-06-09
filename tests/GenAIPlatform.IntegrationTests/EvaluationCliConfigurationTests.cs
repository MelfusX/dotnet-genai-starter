using Microsoft.Extensions.Configuration;

namespace GenAIPlatform.IntegrationTests;

public sealed class EvaluationCliConfigurationTests
{
    [Fact]
    public void EvaluationCliHost_LoadsBundledAppSettingsBeforeEnvironmentAndCommandLine()
    {
        var contentRoot = Directory.CreateTempSubdirectory("genai-eval-cli-config-").FullName;
        try
        {
            File.WriteAllText(
                Path.Combine(contentRoot, "appsettings.json"),
                """
                {
                  "GenAIPlatform": {
                    "ModelGateway": {
                      "Provider": "JsonProvider"
                    }
                  }
                }
                """);

            var builder = global::EvaluationCliHost.CreateBuilder(
                ["--GenAIPlatform:ModelGateway:Provider=CommandLineProvider"],
                contentRoot);

            Assert.Equal(
                "CommandLineProvider",
                builder.Configuration["GenAIPlatform:ModelGateway:Provider"]);

            var sources = ((IConfigurationBuilder)builder.Configuration).Sources;
            var appSettingsIndex = FindLastSourceIndex(sources, "JsonConfigurationSource", "appsettings.json");
            var environmentIndex = FindLastSourceIndex(sources, "EnvironmentVariablesConfigurationSource");
            var commandLineIndex = FindLastSourceIndex(sources, "CommandLineConfigurationSource");

            Assert.True(appSettingsIndex >= 0);
            Assert.True(environmentIndex > appSettingsIndex);
            Assert.True(commandLineIndex > appSettingsIndex);
        }
        finally
        {
            Directory.Delete(contentRoot, recursive: true);
        }
    }

    [Fact]
    public void EvaluationCliHost_RemovesRunSubcommandBeforeCommandLineConfiguration()
    {
        var contentRoot = Directory.CreateTempSubdirectory("genai-eval-cli-command-").FullName;
        try
        {
            File.WriteAllText(
                Path.Combine(contentRoot, "appsettings.json"),
                """
                {
                  "GenAIPlatform": {
                    "ModelGateway": {
                      "Provider": "JsonProvider"
                    }
                  }
                }
                """);

            var builder = global::EvaluationCliHost.CreateBuilder(
                ["run", "--GenAIPlatform:ModelGateway:Provider=CommandLineProvider"],
                contentRoot);

            Assert.Equal(
                "CommandLineProvider",
                builder.Configuration["GenAIPlatform:ModelGateway:Provider"]);
        }
        finally
        {
            Directory.Delete(contentRoot, recursive: true);
        }
    }

    private static int FindLastSourceIndex(
        IList<IConfigurationSource> sources,
        string typeName,
        string? pathSuffix = null)
    {
        for (var index = sources.Count - 1; index >= 0; index--)
        {
            var source = sources[index];
            if (!string.Equals(source.GetType().Name, typeName, StringComparison.Ordinal))
            {
                continue;
            }

            if (pathSuffix is null)
            {
                return index;
            }

            var pathProperty = source.GetType().GetProperty("Path");
            if (pathProperty?.GetValue(source) is string path &&
                path.EndsWith(pathSuffix, StringComparison.OrdinalIgnoreCase))
            {
                return index;
            }
        }

        return -1;
    }
}
