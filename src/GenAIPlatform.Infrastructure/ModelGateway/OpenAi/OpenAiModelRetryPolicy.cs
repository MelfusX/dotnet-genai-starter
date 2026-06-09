using System.Net;
using GenAIPlatform.Infrastructure.Configuration;

namespace GenAIPlatform.Infrastructure.ModelGateway.OpenAi;

internal sealed class OpenAiModelRetryPolicy
{
    public bool ShouldRetry(HttpStatusCode statusCode)
    {
        var statusCodeValue = (int)statusCode;
        return statusCode is HttpStatusCode.RequestTimeout or
               HttpStatusCode.TooManyRequests ||
               statusCodeValue >= 500;
    }

    public Task DelayBeforeRetryAsync(
        OpenAiCompatibleModelClientOptions clientOptions,
        HttpResponseMessage? response,
        int attempt,
        CancellationToken cancellationToken)
    {
        var retryAfter = response?.Headers.RetryAfter?.Delta;
        if (retryAfter is { } retryAfterDelay && retryAfterDelay > TimeSpan.Zero)
        {
            return Task.Delay(ClampRetryDelay(retryAfterDelay), cancellationToken);
        }

        var baseDelayMilliseconds = Math.Max(1, clientOptions.RetryBaseDelayMilliseconds);
        var exponentialDelayMilliseconds = baseDelayMilliseconds * Math.Pow(2, attempt);
        return Task.Delay(
            ClampRetryDelay(TimeSpan.FromMilliseconds(exponentialDelayMilliseconds)),
            cancellationToken);
    }

    private static TimeSpan ClampRetryDelay(TimeSpan delay)
    {
        var maxDelay = TimeSpan.FromSeconds(5);
        return delay > maxDelay ? maxDelay : delay;
    }
}
