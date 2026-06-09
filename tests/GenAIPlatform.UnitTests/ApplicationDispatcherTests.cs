using GenAIPlatform.Application.Core.Dispatching;
using Microsoft.Extensions.DependencyInjection;

namespace GenAIPlatform.UnitTests;

public sealed class ApplicationDispatcherTests
{
    [Fact]
    public async Task DispatchAsync_RunsHandlerThroughRegisteredPipelineBehaviors()
    {
        var services = new ServiceCollection();
        services.AddScoped<IApplicationDispatcher, ApplicationDispatcher>();
        services.AddScoped<IRequestHandler<PingRequest, string>, PingHandler>();
        services.AddScoped<IPipelineBehavior<PingRequest, string>, SuffixBehavior>();

        using var provider = services.BuildServiceProvider();
        var dispatcher = provider.GetRequiredService<IApplicationDispatcher>();

        var result = await dispatcher.DispatchAsync<PingRequest, string>(
            new PingRequest("hello"),
            CancellationToken.None);

        Assert.Equal("hello handled through-pipeline", result);
    }

    private sealed record PingRequest(string Message) : IRequest<string>;

    private sealed class PingHandler : IRequestHandler<PingRequest, string>
    {
        public Task<string> HandleAsync(PingRequest request, CancellationToken cancellationToken) =>
            Task.FromResult($"{request.Message} handled");
    }

    private sealed class SuffixBehavior : IPipelineBehavior<PingRequest, string>
    {
        public async Task<string> HandleAsync(
            PingRequest request,
            RequestHandlerDelegate<string> next,
            CancellationToken cancellationToken)
        {
            var result = await next();
            return $"{result} through-pipeline";
        }
    }
}
