using Microsoft.AspNetCore.Identity;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using OperationalWorkspaceInfrastructure.SecurityInfrastructure;

namespace OperationalWorkspaceAPI.Authentication;

public class JwtAuthProvider : IAuthProvider
{
    private readonly IAccountRepository _accounts;

    private readonly JwtTokenService _jwt;

    public JwtAuthProvider(
        IAccountRepository accounts,
        JwtTokenService jwt)
    {
        _accounts = accounts;

        _jwt = jwt;
    }

    public async Task<AuthResult> AuthenticateAsync(
        string username,
        string password)
    {
        // ======================================================
        // LOAD USER
        // ======================================================

        var user =
            await _accounts.FindAccountByUsernameAsync(
                username);

        if (user == null)
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "Invalid credentials"
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

        if (result != PasswordVerificationResult.Success)
        {
            return new AuthResult
            {
                Success = false,
                ErrorMessage = "Invalid credentials"
            };
        }

        // ======================================================
        // GENERATE JWT
        // ======================================================

        var token =
            _jwt.GenerateToken(user);

        // ======================================================
        // RETURN SUCCESS
        // ======================================================

        return new AuthResult
        {
            Success = true,
            Token = token
        };
    }
}