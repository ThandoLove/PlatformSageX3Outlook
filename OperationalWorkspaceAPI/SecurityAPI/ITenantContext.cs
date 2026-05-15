namespace OperationalWorkspaceAPI.SecurityAPI;


public interface ITenantContext
{
    string Company { get; }
    string Dataset { get; }
}