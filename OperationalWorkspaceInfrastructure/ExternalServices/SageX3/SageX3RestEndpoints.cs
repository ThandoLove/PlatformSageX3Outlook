using System;

namespace OperationalWorkspaceInfrastructure.ExternalServices.SageX3;

/// <summary>
/// Sage X3 Syracuse REST Web Services paths (api1/x3/erp/{folder}/...).
/// See: Sage X3 Web Services REST API — entity names match X3 object codes.
/// </summary>
public static class SageX3RestEndpoints
{
    // ── Entity codes (X3 object names) ──────────────────────────────────────
    public const string Customer = "BPCUSTOMER";
    public const string Supplier = "BPSUPPLIER";
    public const string Contact = "CONTACT";
    public const string SalesOrder = "SORDER";
    public const string SalesQuote = "SQUOTE";
    public const string SalesInvoice = "SIH";

    // ── URL builders (relative to RestBaseUrl, e.g. …/api1/x3/erp/SEED) ─────

    public static string Query(string entity, int? count = null, string? where = null)
    {
        var url = $"{entity}?representation={entity}.$query";
        if (count.HasValue)
            url += $"&count={count.Value}";
        if (!string.IsNullOrWhiteSpace(where))
            url += $"&where={Uri.EscapeDataString(where)}";
        return url;
    }

    public static string Details(string entity, string key) =>
        $"{entity}('{EscapeKey(key)}')?representation={entity}.$details";

    public static string Create(string entity) =>
        $"{entity}?representation={entity}.$create";

    public static string Update(string entity, string key) =>
        $"{entity}('{EscapeKey(key)}')?representation={entity}.$edit";

    // ── Common queries ──────────────────────────────────────────────────────

    public static string CustomerByCode(string bpCode) =>
        Details(Customer, bpCode);

    public static string CustomerQuery(int count = 100) =>
        Query(Customer, count);

    public static string ContactByEmail(string email) =>
        Query(Contact, count: 1, where: $"WEB eq '{EscapeOData(email)}'");

    public static string SalesOrderByNumber(string orderNumber) =>
        Details(SalesOrder, orderNumber);

    public static string OpenSalesOrdersQuery(int count = 50) =>
        Query(SalesOrder, count, "ORDSTA eq '1'"); // 1 = Open (site-specific; adjust when connected)

    // ── Invoice REST Endpoints (🚀 RESTORED) ───────────────────────────────
    public static string SalesInvoiceByNumber(string invoiceNumber) =>
        Details(SalesInvoice, invoiceNumber);

    public static string SalesInvoicesQuery(int count = 100) =>
        Query(SalesInvoice, count);

    // =========================================================================
    // 🗑️ CRITICAL DECOMMISSIONING COMPLETED
    // All local relational Stock, Warehouse site filters, and stock adjustment 
    // endpoint builders have been completely scrubbed from this dictionary! [INDEX]
    // =========================================================================

    private static string EscapeKey(string key) =>
        key.Replace("'", "''", StringComparison.Ordinal);

    private static string EscapeOData(string value) =>
        value.Replace("'", "''", StringComparison.Ordinal);
}
