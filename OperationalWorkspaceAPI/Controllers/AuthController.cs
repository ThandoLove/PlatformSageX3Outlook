using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceInfrastructure.Persistence.Repositories;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OperationalWorkspaceAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ApiController
{
    private readonly IAccountRepository _repo;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAccountRepository repo, IConfiguration config, ILogger<AuthController> logger)
    {
        _repo = repo;
        _config = config;
        _logger = logger;
    }

    /// <summary>
    /// Authenticates a user using LoginRequestDto and returns a JWT + UserDto.
    /// URL: POST api/v1/Auth/login
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        // 1. Basic Validation
        if (string.IsNullOrWhiteSpace(dto?.Username) || string.IsNullOrWhiteSpace(dto?.Password))
        {
            return Failure("Username and password are required.", 401);
        }

        // 2. Database Lookup: Ask the Librarian for the REAL account info (User Entity)
        var userAccount = await _repo.FindAccountByUsernameAsync(dto.Username);

        // 3. Credential Verification (Comparing typed password to Database hash)
        if (userAccount != null && dto.Password == userAccount.PasswordHash)
        {
            // 4. Generate the JWT "Passport"
            var token = GenerateJwtToken(userAccount);

            // 5. Create the "Identity Card" (UserDto) for the Blazor UI
            var userDto = new UserDto
            {
                Id = userAccount.Id.ToString(),
                Name = userAccount.Username,
                Role = userAccount.Role,
                Environment = "Production"
            };

            _logger.LogInformation("LOGIN SUCCESS: User {User} authenticated. TraceId: {TraceId}",
                dto.Username, TraceId);

            // 6. Return standard envelope: success, data (token + user), traceId, timestamp
            return Success(new { token, user = userDto });
        }

        // 7. Log Security Failure
        _logger.LogWarning("LOGIN FAILED: Invalid attempt for user {User}. TraceId: {TraceId}",
            dto.Username, TraceId);

        return Failure("Invalid username or password.", 401);
    }

    private string GenerateJwtToken(LoginUser user)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var keyString = jwtSettings["Key"] ?? "A_Very_Long_Default_Key_For_Development_32_Chars";

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role), // REQUIRED for your RbacMiddleware
            new Claim("username", user.Username),
            new Claim("generated_at", DateTime.UtcNow.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiryHours = Convert.ToDouble(jwtSettings["ExpiryHours"] ?? "8");
        var expires = DateTime.UtcNow.AddHours(expiryHours);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
