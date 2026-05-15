using OperationalWorkspaceAPI.SecurityAPI;

namespace OperationalWorkspaceAPI.SecurityAPI;

public class TenantContext : ITenantContext
{
    private readonly ISecurityContext _security;

    public TenantContext(ISecurityContext security)
    {
        _security = security;
    }

    public string Company =>
        _security.Company
        ?? throw new UnauthorizedAccessException(
            "Missing Company claim");

    public string Dataset =>
        _security.Dataset
        ?? throw new UnauthorizedAccessException(
            "Missing Dataset claim");
}