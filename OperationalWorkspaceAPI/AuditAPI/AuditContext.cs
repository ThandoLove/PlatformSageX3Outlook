using OperationalWorkspaceAPI.SecurityAPI;

namespace OperationalWorkspaceAPI.AuditAPI;

public class AuditContext
{
    private readonly ISecurityContext _security;

    public AuditContext(
        ISecurityContext security)
    {
        _security = security;
    }

    public AuditEvent Create(
        AuditEventType action,
        string entity = "",
        string entityId = "")
    {
        return new AuditEvent
        {
            UserId = _security.UserId,

            Email = _security.Email,

            Company = _security.Company,

            Dataset = _security.Dataset,

            Action = action,

            Entity = entity,

            EntityId = entityId,

            TimestampUtc = DateTime.UtcNow,

            Success = true
        };
    }
}