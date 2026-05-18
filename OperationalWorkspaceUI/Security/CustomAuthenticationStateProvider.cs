
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Majorsoft.Blazor.Extensions.BrowserStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace OperationalWorkspaceUI.Security;

public class CustomAuthenticationStateProvider
    : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;

    private readonly ClaimsPrincipal _anonymous =
        new(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(
        ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    // ======================================================
    // GET CURRENT AUTH STATE
    // ======================================================

    public override async Task<AuthenticationState>
        GetAuthenticationStateAsync()
    {
        try
        {
            var token =
                await _localStorage.GetItemAsync<string>(
                    "authToken");

            if (string.IsNullOrWhiteSpace(token))
            {
                return new AuthenticationState(
                    _anonymous);
            }

            var claims =
                ParseClaimsFromJwt(token);

            // Invalid or expired token
            if (!claims.Any())
            {
                await _localStorage.RemoveItemAsync(
                    "authToken");

                return new AuthenticationState(
                    _anonymous);
            }

            var identity =
                new ClaimsIdentity(
                    claims,
                    authenticationType: "jwt");

            var user =
                new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch
        {
            return new AuthenticationState(
                _anonymous);
        }
    }

    // ======================================================
    // LOGIN NOTIFICATION
    // ======================================================

    public void NotifyUserAuthentication(
        string token)
    {
        var claims =
            ParseClaimsFromJwt(token);

        var identity =
            new ClaimsIdentity(
                claims,
                authenticationType: "jwt");

        var user =
            new ClaimsPrincipal(identity);

        var authState =
            Task.FromResult(
                new AuthenticationState(user));

        NotifyAuthenticationStateChanged(
            authState);
    }

    // ======================================================
    // LOGOUT NOTIFICATION
    // ======================================================

    public void NotifyUserLogout()
    {
        var authState =
            Task.FromResult(
                new AuthenticationState(
                    _anonymous));

        NotifyAuthenticationStateChanged(
            authState);
    }

    // ======================================================
    // JWT CLAIM PARSER
    // ======================================================

    private IEnumerable<Claim> ParseClaimsFromJwt(
        string jwt)
    {
        var claims =
            new List<Claim>();

        try
        {
            var segments = jwt.Split('.');

            // JWT must contain:
            // Header.Payload.Signature
            if (segments.Length != 3)
            {
                return claims;
            }

            var payload =
                segments[1];

            var jsonBytes =
                ParseBase64WithoutPadding(
                    payload);

            var keyValuePairs =
                JsonSerializer.Deserialize<
                    Dictionary<string, object>>(
                        jsonBytes);

            if (keyValuePairs == null)
            {
                return claims;
            }

            // ======================================================
            // EXPIRATION VALIDATION
            // ======================================================

            if (keyValuePairs.TryGetValue(
                JwtRegisteredClaimNames.Exp,
                out var expValue))
            {
                if (long.TryParse(
                    expValue.ToString(),
                    out var expUnix))
                {
                    var expiration =
                        DateTimeOffset
                            .FromUnixTimeSeconds(
                                expUnix);

                    // Expired token
                    if (expiration <= DateTimeOffset.UtcNow)
                    {
                        return claims;
                    }
                }
            }

            // ======================================================
            // CLAIM MAPPING
            // ======================================================

            foreach (var kvp in keyValuePairs)
            {
                if (kvp.Value == null)
                {
                    continue;
                }

                // ======================================================
                // ROLE ARRAY HANDLING
                // ======================================================

                if (kvp.Key == "role"
                    && kvp.Value.ToString()?.Trim()
                        .StartsWith("[") == true)
                {
                    var roles =
                        JsonSerializer.Deserialize<
                            List<string>>(
                                kvp.Value.ToString()!);

                    if (roles != null)
                    {
                        foreach (var role in roles)
                        {
                            claims.Add(
                                new Claim(
                                    ClaimTypes.Role,
                                    role));
                        }
                    }

                    continue;
                }

                // ======================================================
                // STANDARD JWT CLAIM MAPPING
                // ======================================================

                var claimType =
                    kvp.Key switch
                    {
                        JwtRegisteredClaimNames.Sub =>
                            ClaimTypes.NameIdentifier,

                        JwtRegisteredClaimNames.UniqueName =>
                            ClaimTypes.Name,

                        "role" =>
                            ClaimTypes.Role,

                        _ =>
                            kvp.Key
                    };

                claims.Add(
                    new Claim(
                        claimType,
                        kvp.Value.ToString()!));
            }

            return claims;
        }
        catch
        {
            return new List<Claim>();
        }
    }

    // ======================================================
    // BASE64 DECODER
    // ======================================================

    private byte[] ParseBase64WithoutPadding(
        string base64)
    {
        switch (base64.Length % 4)
        {
            case 2:
                base64 += "==";
                break;

            case 3:
                base64 += "=";
                break;
        }

        return Convert.FromBase64String(
            base64);
    }
}

