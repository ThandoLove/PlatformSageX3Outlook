using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace OperationalWorkspaceUI.Security;

public class CustomAuthenticationStateProvider
    : AuthenticationStateProvider
{
    private readonly ClaimsPrincipal _anonymous =
        new(new ClaimsIdentity());

    private ClaimsPrincipal _currentUser =
        new(new ClaimsIdentity());

    // ======================================================
    // GET CURRENT AUTH STATE
    // ======================================================

    public override Task<AuthenticationState>
        GetAuthenticationStateAsync()
    {
        return Task.FromResult(
            new AuthenticationState(_currentUser));
    }

    // ======================================================
    // LOGIN NOTIFICATION
    // ======================================================

    public void NotifyUserAuthentication(
        string token)
    {
        var handler =
            new JwtSecurityTokenHandler();

        var jwt =
            handler.ReadJwtToken(token);

        var identity =
            new ClaimsIdentity(
                jwt.Claims,
                authenticationType: "jwt");

        _currentUser =
            new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(
            Task.FromResult(
                new AuthenticationState(_currentUser)));
    }

    // ======================================================
    // LOGOUT NOTIFICATION
    // ======================================================

    public void NotifyUserLogout()
    {
        _currentUser = _anonymous;

        NotifyAuthenticationStateChanged(
            Task.FromResult(
                new AuthenticationState(_anonymous)));
    }
}