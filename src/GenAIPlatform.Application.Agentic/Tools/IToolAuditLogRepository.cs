using GenAIPlatform.Domain.Agentic;

namespace GenAIPlatform.Application.Agentic.Tools;

public interface IToolAuditLogRepository
{
    Task AddAsync(ToolAuditLogEntry entry, CancellationToken cancellationToken);
}
