using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.DTOs;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OperationalWorkspaceInfrastructure.SecurityInfrastructure;

public class JwtTokenService
{
    private readonly IConfiguration _config;

    public JwtTokenService(
        IConfiguration config)
    {
        _config = config;
    }

    // ======================================================
    // GENERATE ACCESS + REFRESH TOKENS
    // ======================================================

    public AuthTokenResponseDto GenerateTokens(
        UserAccount user)
    {
        var jwt =
            _config.GetSection("Jwt");

        // ======================================================
        // SECURITY KEY
        // ======================================================

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    jwt["Key"]
                    ?? "fallback_secret_key_32_chars"));

        // ======================================================
        // SIGNING CREDS
        // ======================================================

        var creds =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        // ======================================================
        // SHORT-LIVED ACCESS TOKEN
        // ======================================================

        var expires =
            DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(
                    jwt["AccessTokenMinutes"]
                    ?? "30"));

        // ======================================================
        // CLAIMS
        // ======================================================

        var claims =
            new List<Claim>
            {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    user.Username),

                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString()),

                new Claim(
                    "userId",
                    user.Id.ToString()),

                new Claim(
                    "username",
                    user.Username),

                new Claim(
                    ClaimTypes.Role,
                    user.Role),

                new Claim(
                    "email",
                    user.Email ?? ""),

                new Claim(
                    "isActive",
                    user.IsActive.ToString())
            };

        // ======================================================
        // BUILD JWT
        // ======================================================

        var token =
            new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

        // ======================================================
        // SERIALIZE ACCESS TOKEN
        // ======================================================

        var accessToken =
            new JwtSecurityTokenHandler()
                .WriteToken(token);

        // ======================================================
        // GENERATE REFRESH TOKEN
        // ======================================================

        var refreshToken =
            GenerateRefreshToken();

        // ======================================================
        // RETURN TOKEN RESPONSE
        // ======================================================

        return new AuthTokenResponseDto
        {
            AccessToken = accessToken,

            RefreshToken = refreshToken,

            ExpiresAtUtc = expires
        };
    }

    // ======================================================
    // SECURE REFRESH TOKEN GENERATOR
    // ======================================================

    private string GenerateRefreshToken()
    {
        var randomBytes =
            new byte[64];

        using var rng =
            RandomNumberGenerator.Create();

        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(
            randomBytes);
    }
}