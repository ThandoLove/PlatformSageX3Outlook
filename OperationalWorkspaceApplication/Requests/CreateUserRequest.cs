
using System.ComponentModel.DataAnnotations;

namespace OperationalWorkspaceApplication.Requests;

public sealed record CreateUserRequest(
    [Required, StringLength(100)] string Name,
    [Required, EmailAddress] string Email,
    [Required] string Role,
    [Required] string Environment
);