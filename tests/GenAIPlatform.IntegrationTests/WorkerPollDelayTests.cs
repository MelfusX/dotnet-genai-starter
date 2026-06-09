using GenAIPlatform.Worker;

namespace GenAIPlatform.IntegrationTests;

public sealed class WorkerPollDelayTests
{
    [Theory]
    [InlineData(10, 0, 0.0, 8)]
    [InlineData(10, 0, 1.0, 12)]
    [InlineData(10, 3, 0.5, 40)]
    [InlineData(10, 99, 0.5, 40)]
    public void Calculate_AppliesBoundedJitterAndErrorBackoff(
        int pollIntervalSeconds,
        int consecutiveErrors,
        double randomValue,
        double expectedSeconds)
    {
        var delay = WorkerPollDelay.Calculate(
            pollIntervalSeconds,
            consecutiveErrors,
            randomValue);

        Assert.Equal(TimeSpan.FromSeconds(expectedSeconds), delay);
    }
}
