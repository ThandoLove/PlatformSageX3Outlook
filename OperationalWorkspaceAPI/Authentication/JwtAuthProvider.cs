using Microsoft.AspNetCore.Identity;

using OperationalWorkspaceApplication.DTOs;
using OperationalWorkspaceApplication.Interfaces.IRepository;

using OperationalWorkspaceInfrastructure.SecurityInfrastructure;

namespace OperationalWorkspaceAPI.Authentication;

public class JwtAuthProvider : IAuthProvider
{
    // ======================================================
    // DEPENDENCIES
    // ======================================================

    private readonly IAccountRepository _accounts;

    private readonly JwtTokenService _jwt;

    // ======================================================
    // CONSTRUCTOR
    // ======================================================

    public JwtAuthProvider(
        IAccountRepository accounts,
        JwtTokenService jwt)
    {
        _accounts = accounts;

        _jwt = jwt;
    }

    // ======================================================
    // AUTHENTICATE USER
    // ======================================================

    public async Task<AuthResult> AuthenticateAsync(
        string username,
        string password)
    {
        // ======================================================
        // LOAD USER
        // ======================================================

        var user =
            await _accounts
                .FindAccountByUsernameAsync(
                    username);

        if (user == null)
        {
            return new AuthResult
            {
                Success = false,

                ErrorMessage =
                    "Invalid credentials"
            };
        }

        // ======================================================
        // VERIFY PASSWORD
        // ======================================================

        var hasher =
            new PasswordHasher<dynamic>();

        var result =
            hasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password);

        if (result !=
            PasswordVerificationResult.Success)
        {
            return new AuthResult
            {
                Success = false,

                ErrorMessage =
                    "Invalid credentials"
            };
        }

        // ======================================================
        // GENERATE ACCESS + REFRESH TOKENS
        // ======================================================

        var tokens =
            _jwt.GenerateTokens(user);

        // ======================================================
        // RETURN SUCCESS
        // ======================================================

        return new AuthResult
        {
            Success = true,

            AccessToken =
                tokens.AccessToken,

            RefreshToken =
                tokens.RefreshToken,

            ExpiresAtUtc =
                tokens.ExpiresAtUtc
        };
    }
}