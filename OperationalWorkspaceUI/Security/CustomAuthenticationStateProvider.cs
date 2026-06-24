using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Majorsoft.Blazor.Extensions.BrowserStorage;

namespace OperationalWorkspaceUI.Security;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token =
                await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(_anonymous);
            }

            var claims = ParseClaimsFromJwt(token).ToList();

            if (IsExpired(claims))
            {
                return new AuthenticationState(_anonymous);
            }

            var identity =
                new ClaimsIdentity(claims, "jwt");

            _currentUser =
                new ClaimsPrincipal(identity);

            return new AuthenticationState(_currentUser);
        }
        catch
        {
            return new AuthenticationState(_anonymous);
        }
    }

    public void NotifyUserAuthentication(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            NotifyUserLogout();
            return;
        }

        var claims = ParseClaimsFromJwt(token).ToList();
        if (IsExpired(claims))
        {
            NotifyUserLogout();
            return;
        }

        var identity = new ClaimsIdentity(claims, authenticationType: "jwt");
        _currentUser = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(_currentUser)));
    }

    public void NotifyUserLogout()
    {
        _currentUser = _anonymous;

        NotifyAuthenticationStateChanged(
            Task.FromResult(new AuthenticationState(_anonymous)));
    }

    // Lightweight native JWT payload extractor
    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var segments = jwt.Split('.');
        if (segments.Length < 2) return Enumerable.Empty<Claim>();

        var payload = segments[1];
        payload = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=')
                         .Replace('-', '+')
                         .Replace('_', '/');

        var jsonBytes = Convert.FromBase64String(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs == null) return Enumerable.Empty<Claim>();

        return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? ""));
    }

    private static bool IsTokenExpired(ClaimsPrincipal user)
    {
        if (user.Identity?.IsAuthenticated != true) return false;
        return IsExpired(user.Claims);
    }

    private static bool IsExpired(IEnumerable<Claim> claims)
    {
        var expClaim = claims.FirstOrDefault(c => c.Type is "exp" or "Exp");
        if (expClaim == null || !long.TryParse(expClaim.Value, out var expUnix))
            return false;

        var expiry = DateTimeOffset.FromUnixTimeSeconds(expUnix);
        return expiry <= DateTimeOffset.UtcNow;
    }

    private readonly ILocalStorageService _localStorage;

    public CustomAuthenticationStateProvider(
        ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }
}
