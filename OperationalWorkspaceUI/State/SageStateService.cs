
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace OperationalWorkspaceUI.State;

public class SageStateService
{
    private readonly AuthenticationStateProvider _authStateProvider;

    public SageStateService(AuthenticationStateProvider authStateProvider)
    {
        _authStateProvider = authStateProvider;
    }

    // --- USER & ENVIRONMENT ---
    public string Environment { get; set; } = "SEED - Production (Syracuse)";
    public bool IsConnected { get; set; } = true;

    public async Task<string> GetCurrentUserName()
    {
        var authState = await _authStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        return user.Identity?.IsAuthenticated == true
            ? user.FindFirst(ClaimTypes.Name)?.Value ?? user.Identity.Name ?? "Sage User"
            : "Guest User";
    }

    // --- BUSINESS DATA (Exactly like the image) ---
    public decimal OverdueInvoices { get; set; } = 12500.00m;
    public int ActiveTickets { get; set; } = 5; // Replaced Opportunities
    public string CreditLimit { get; set; } = "50,000";
    public double CreditUsagePercent { get; set; } = 75;

    // --- KNOWLEDGE BASE ---
    public List<KbArticle> Articles { get; set; } = new()
    {
        new("Pricing Guidelines", "GUIDE-001", "Success"),
        new("Product Documentation", "DOC-002", "Warning"),
        new("Case Studies", "CASE-003", "Accent"),
        new("FAQ", "FAQ-004", "Warning")
    };

    // --- REPORTS ---
    public List<string> ReportList { get; set; } = new()
    {
        "Aged Receivables (Invoices)",
        "Open Tickets Summary",
        "Daily Sales Order Log"
    };

    // --- SALES HISTORY CHART ---
    public int[] GetLatestSalesHistory()
    {
        var rnd = new Random();
        return new int[] { 12000, 15000, 9000, 12500, 18000, 14000 + rnd.Next(-1000, 5000) };
    }
}

public record KbArticle(string Title, string Code, string Color);
