namespace GenAIPlatform.Worker;

public static class WorkerPollDelay
{
    private const double JitterRatio = 0.2;
    private const int MaxErrorMultiplier = 4;

    public static TimeSpan Calculate(
        int pollIntervalSeconds,
        int consecutiveErrors,
        double randomValue)
    {
        var baseDelay = TimeSpan.FromSeconds(Math.Max(1, pollIntervalSeconds));
        var errorMultiplier = Math.Min(
            MaxErrorMultiplier,
            1 + Math.Max(0, consecutiveErrors));
        var jitter = 1 + ((Math.Clamp(randomValue, 0, 1) * 2) - 1) * JitterRatio;

        return TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds * errorMultiplier * jitter);
    }
}
