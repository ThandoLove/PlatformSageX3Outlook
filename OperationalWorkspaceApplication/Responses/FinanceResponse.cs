using OperationalWorkspaceApplication.DTOs;


namespace OperationalWorkspaceApplication.Responses
{
    public sealed record AgingReportResponse(AgingSummaryDto Summary);
    public sealed record RegisterPaymentResponse(bool Success, string V);
}
