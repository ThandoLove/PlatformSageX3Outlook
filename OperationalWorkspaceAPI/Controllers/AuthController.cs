using Microsoft.AspNetCore.Mvc;

using OperationalWorkspaceAPI.Controllers;
using System.Threading.Tasks;
using OperationalWorkspaceApplication.DTOs;
using System;

namespace OperationalWorkspaceAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ApiController
{
    // Minimal mock authentication for development.
    // Replace with your real identity provider / token service in production.

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto?.Username) || string.IsNullOrWhiteSpace(dto?.Password))
            return Failure("Invalid credentials.", 401);

        // Very simple check for local development. Change this.
        if (dto.Username == "admin" && dto.Password == "password")
        {
            var token = Guid.NewGuid().ToString();
            return Success(new { token });
        }

        return Failure("Invalid username or password.", 401);
    }
}
