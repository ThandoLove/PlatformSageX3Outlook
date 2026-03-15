using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;



namespace OperationalWorkspaceApplication.Interfaces.IServices;


public interface IBusinessPartnerService
{
    Task<BusinessPartnersResponse?> GetSnapshotAsync(
        GetBusinessPartnerSnapshotRequest request,
        CancellationToken cancellationToken);

    Task<UpdateCreditLimitResponse> UpdateCreditLimitAsync(
        UpdateCreditLimitRequest request,
        CancellationToken cancellationToken);
}


