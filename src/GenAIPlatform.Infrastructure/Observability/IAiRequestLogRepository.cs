using GenAIPlatform.Domain.Observability;

namespace GenAIPlatform.Infrastructure.Observability;

public interface IAiRequestLogRepository
{
    /// <summary>
    /// Persists sanitized model request telemetry. Implementations must not store full rendered prompts,
    /// response bodies, document chunk text, secrets or provider credentials.
    /// </summary>
    Task AddAsync(
        AiRequestLogEntry entry,
        CancellationToken cancellationToken);
}
