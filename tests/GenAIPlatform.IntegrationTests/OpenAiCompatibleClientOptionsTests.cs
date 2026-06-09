using GenAIPlatform.Infrastructure.Configuration;

namespace GenAIPlatform.IntegrationTests;

public sealed class OpenAiCompatibleClientOptionsTests
{
    [Theory]
    [InlineData("/v1/chat/completions", "https://api.openai.com/v1/chat/completions")]
    [InlineData("v1/chat/completions", "https://api.openai.com/v1/chat/completions")]
    public void ModelClientOptions_AcceptsRelativeEndpointPath(
        string endpointPath,
        string expectedEndpoint)
    {
        var options = new OpenAiCompatibleModelClientOptions
        {
            ApiKey = "test-api-key",
            ChatCompletionsPath = endpointPath
        };

        var created = options.TryCreateEndpointUri(out var endpointUri);

        Assert.True(created);
        Assert.True(options.IsValid());
        Assert.Equal(expectedEndpoint, endpointUri!.ToString().TrimEnd('/'));
    }

    [Theory]
    [InlineData("/v1/embeddings", "https://api.openai.com/v1/embeddings")]
    [InlineData("v1/embeddings", "https://api.openai.com/v1/embeddings")]
    public void EmbeddingClientOptions_AcceptsRelativeEndpointPath(
        string endpointPath,
        string expectedEndpoint)
    {
        var options = new OpenAiCompatibleEmbeddingClientOptions
        {
            ApiKey = "test-api-key",
            EmbeddingsPath = endpointPath
        };

        var created = options.TryCreateEndpointUri(out var endpointUri);

        Assert.True(created);
        Assert.True(options.IsValid());
        Assert.Equal(expectedEndpoint, endpointUri!.ToString().TrimEnd('/'));
    }

    [Theory]
    [InlineData("https://provider.example/v1/chat/completions")]
    [InlineData("//provider.example/v1/chat/completions")]
    public void ModelClientOptions_RejectsAbsoluteEndpointPath(string endpointPath)
    {
        var options = new OpenAiCompatibleModelClientOptions
        {
            ApiKey = "test-api-key",
            ChatCompletionsPath = endpointPath
        };

        Assert.False(options.TryCreateEndpointUri(out _));
        Assert.False(options.IsValid());
    }

    [Theory]
    [InlineData("https://provider.example/v1/embeddings")]
    [InlineData("//provider.example/v1/embeddings")]
    public void EmbeddingClientOptions_RejectsAbsoluteEndpointPath(string endpointPath)
    {
        var options = new OpenAiCompatibleEmbeddingClientOptions
        {
            ApiKey = "test-api-key",
            EmbeddingsPath = endpointPath
        };

        Assert.False(options.TryCreateEndpointUri(out _));
        Assert.False(options.IsValid());
    }
}
