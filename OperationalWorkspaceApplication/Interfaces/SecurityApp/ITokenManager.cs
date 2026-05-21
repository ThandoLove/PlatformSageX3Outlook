using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Interfaces.SecurityApp

{
    /// <summary>
    /// Contract driving the secure stateless JWT token lifecycle management engine, managing generation, sliding window rotation, and tampering validation.
    /// </summary>
    public interface ITokenManager
    {
        Task<(string AccessToken, string RefreshToken)> GenerateTokenPairAsync(string email, string ipAddress);
        Task<(string AccessToken, string RefreshToken)> RotateRefreshTokenAsync(string expiredAccessToken, string currentRefreshToken, string ipAddress);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
