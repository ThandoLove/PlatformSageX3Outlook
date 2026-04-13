namespace OperationalWorkspaceApplication.DTOs;

public class LoginRequestDto
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
  
     public int Id { get; set; }
       
     public string Role { get; set; } = "User";
    public string? Password { get; set; }
}
