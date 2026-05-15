using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspace.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace OperationalWorkspaceAPI.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ApiController
{
    private readonly IAccountRepository _repo;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IAccountRepository repo,
        IConfiguration config,
        ILogger<AuthController> logger)
    {
        _repo = repo;
        _config = config;
        _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var traceId = HttpContext.TraceIdentifier;

        if (string.IsNullOrWhiteSpace(dto?.Username) || string.IsNullOrWhiteSpace(dto?.Password))
            return Failure("Username and password are required.", 401);

        var userAccount = await _repo.FindAccountByUsernameAsync(dto.Username);

        if (userAccount == null)
        {
            _logger.LogWarning("LOGIN FAILED (NOT FOUND): {User} TraceId:{TraceId}",
                dto.Username, traceId);

            return Failure("Invalid username or password.", 401);
        }

        // ==============================
        // LOCKOUT CHECK
        // ==============================
        if (userAccount.IsLocked &&
            userAccount.LockoutEnd.HasValue &&
            userAccount.LockoutEnd > DateTime.UtcNow)
        {
            return Failure("Account is temporarily locked.", 403);
        }

        if (userAccount.IsLocked &&
            userAccount.LockoutEnd.HasValue &&
            userAccount.LockoutEnd <= DateTime.UtcNow)
        {
            userAccount.IsLocked = false;
            userAccount.FailedLoginAttempts = 0;
            userAccount.LockoutEnd = null;
        }

        // ==============================
        // PASSWORD CHECK
        // ==============================
        var hasher = new PasswordHasher<UserAccount>();

        var result = hasher.VerifyHashedPassword(
            userAccount,
            userAccount.PasswordHash,
            dto.Password
        );

        if (result != PasswordVerificationResult.Success)
        {
            userAccount.FailedLoginAttempts++;

            if (userAccount.FailedLoginAttempts >= 5)
            {
                userAccount.IsLocked = true;
                userAccount.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
            }

            await _repo.UpdateAsync(userAccount);

            return Failure("Invalid username or password.", 401);
        }

        // ==============================
        // SUCCESS RESET
        // ==============================
        userAccount.FailedLoginAttempts = 0;
        userAccount.IsLocked = false;
        userAccount.LockoutEnd = null;
        userAccount.LastLoginAt = DateTime.UtcNow;

        await _repo.UpdateAsync(userAccount);

        // ==============================
        // JWT + REFRESH TOKEN
        // ==============================
        var token = GenerateJwtToken(userAccount);
        var refreshToken = GenerateRefreshToken();

        await _repo.SaveRefreshTokenAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userAccount.Id.ToString(),
            Token = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        });

        var userDto = new UserDto
        {
            Id = userAccount.Id.ToString(),
            Name = userAccount.Username,
            Role = userAccount.Role,
            Environment = "Production"
        };

        _logger.LogInformation("LOGIN SUCCESS: {User} TraceId:{TraceId}",
            dto.Username, traceId);

        return Success(new
        {
            token,
            refreshToken,
            user = userDto
        });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
    {
        var stored = await _repo.GetRefreshTokenAsync(dto.RefreshToken);

        if (stored == null || stored.IsRevoked || stored.ExpiresAt < DateTime.UtcNow)
            return Failure("Invalid refresh token.", 401);

        var userAccount = await _repo.FindAccountByIdAsync(stored.UserId);

        if (userAccount == null)
            return Failure("User not found.", 401);

        // ==============================
        // REVOKE OLD TOKEN (ROTATION)
        // ==============================
        stored.IsRevoked = true;
        await _repo.UpdateRefreshTokenAsync(stored);

        // ==============================
        // NEW TOKENS
        // ==============================
        var newToken = GenerateJwtToken(userAccount);
        var newRefreshToken = GenerateRefreshToken();

        await _repo.SaveRefreshTokenAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userAccount.Id.ToString(),
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        });

        return Success(new
        {
            token = newToken,
            refreshToken = newRefreshToken
        });
    }

    // ==============================
    // JWT GENERATION
    // ==============================
    private string GenerateJwtToken(UserAccount user)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var keyString = jwtSettings["Key"] ?? "A_Very_Long_Default_Key_For_Development_32_Chars";

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("username", user.Username),
            new Claim("userId", user.Id.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddHours(
            Convert.ToDouble(jwtSettings["ExpiryHours"] ?? "8"));

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
    }
}