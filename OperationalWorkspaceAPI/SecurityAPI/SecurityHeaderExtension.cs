using Microsoft.AspNetCore.Builder;
using NetEscapades.AspNetCore.SecurityHeaders;

namespace OperationalWorkspaceAPI.SecurityAPI;

public static class SecurityHeaderExtensions
{
    public static IApplicationBuilder UseEnterpriseSecurityHeaders(this IApplicationBuilder app)
    {
        var policyCollection = new HeaderPolicyCollection();

        // =========================
        // BASIC SECURITY HEADERS
        // (WORKS IN v1.3.1)
        // =========================

        policyCollection.AddCustomHeader("X-Frame-Options", "DENY");
        policyCollection.AddCustomHeader("X-Content-Type-Options", "nosniff");
        policyCollection.AddCustomHeader("Referrer-Policy", "strict-origin-when-cross-origin");

        // =========================
        // CSP (MANUAL STRING MODE)
        // =========================

        policyCollection.AddCustomHeader(
            "Content-Security-Policy",
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
            "style-src 'self' 'unsafe-inline'; " +
            "img-src 'self' data: blob:; " +
            "font-src 'self'; " +
            "connect-src 'self'; " +
            "object-src 'none'; " +
            "frame-ancestors 'none';"
        );

        return app.UseSecurityHeaders(policyCollection);
    }
}