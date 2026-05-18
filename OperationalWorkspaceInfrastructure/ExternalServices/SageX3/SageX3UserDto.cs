
namespace OperationalWorkspaceInfrastructure.ExternalServices.SageX3;

public class SageX3UserDto
{
    public string UserId { get; set; }= string.Empty;
    public string Email { get; set; } = string.Empty;

    public string Company { get; set; }= string.Empty;
    public string Dataset { get; set; }= string.Empty;

    public string Role { get; set; }=string.Empty;

    public bool IsActive { get; set; }
    public List<string> Permissions { get; internal set; } = new List<string>();
    public string Username { get; internal set; } = string.Empty;
}