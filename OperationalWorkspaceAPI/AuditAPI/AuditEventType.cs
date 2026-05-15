namespace OperationalWorkspaceAPI.AuditAPI;


public enum AuditEventType
{
    ApiRequest = 1,
    Login = 2,
    Logout = 3,
    Create = 4,
    Update = 5,
    Delete = 6,
    Export = 7,
    Import = 8,
    Error = 9
}