using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace GenAIPlatform.IntegrationTests;

public sealed class OpenApiContractTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static readonly JsonSerializerOptions BaselineJsonOptions = new()
    {
        WriteIndented = true
    };

    [Fact]
    public async Task DevelopmentOpenApiDocument_MatchesCommittedBaseline()
    {
        using var developmentFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        });
        using var client = developmentFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.GetAsync("/openapi/v1.json", TestContext.Current.CancellationToken);
        var actual = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        var expected = await File.ReadAllTextAsync(BaselinePath(), TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(NormalizeJson(expected), NormalizeJson(actual));
    }

    private static string BaselinePath() =>
        Path.Combine(AppContext.BaseDirectory, "Baselines", "openapi-v1.json");

    private static string NormalizeJson(string json)
    {
        using var document = JsonDocument.Parse(json);
        return JsonSerializer.Serialize(document.RootElement, BaselineJsonOptions);
    }
}
