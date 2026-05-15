using OperationalWorkspaceAPI.AuditAPI;

namespace OperationalWorkspaceAPI.AuditAPI;

public interface IAuditLogger
{
    Task LogAsync(AuditEvent auditEvent);
}