
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;


namespace OperationalWorkspaceApplication.Interfaces.IServices;

public interface IFinancialService
{
    Task<AgingReportResponse> GetAgingReportAsync(
        GetAgingReportRequest request,
        CancellationToken cancellationToken);

    Task<RegisterPaymentResponse> RegisterPaymentAsync(
        RegisterPaymentRequest request,
        CancellationToken cancellationToken);
}