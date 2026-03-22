using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.Requests;
using OperationalWorkspaceApplication.Responses;



namespace OperationalWorkspaceApplication.Interfaces.IServices;


public interface IBusinessPartnerService
{
    Task<int> CountActiveCustomersAsync();
    Task<int> CountNewLeadsTodayAsync();
    Task<int> CountOpenOpportunitiesAsync(string userId);
    Task<int> CountOpenOpportunitiesAsync();
    Task<string> GetRecentInteractionAsync(string userId);
    Task<BusinessPartnersResponse?> GetSnapshotAsync(
        GetBusinessPartnerSnapshotRequest request,
        CancellationToken cancellationToken);
    Task<string?> GetTopCustomerAsync(string userId);
    Task<UpdateCreditLimitResponse> UpdateCreditLimitAsync(
        UpdateCreditLimitRequest request,
        CancellationToken cancellationToken);

    

}


