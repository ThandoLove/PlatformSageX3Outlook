

namespace OperationalWorkspaceApplication.DTOs

{

    public record BusinessPartnerSnapshotDto(
        string BpCode,
        string Company,
        decimal CreditLimit,
        decimal TotalOutstanding,
        int OpenOrderCount,
        int OverdueInvoiceCount,
        decimal OverdueAmount,
        DateTime LastContactDate); // Ensure it has exactly these 8 parameters
}