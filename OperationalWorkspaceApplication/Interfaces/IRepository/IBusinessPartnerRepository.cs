using OperationalWorkspace.Domain.Entities;
using System.Threading;

namespace OperationalWorkspaceApplication.Interfaces.IRepository;


public interface IBusinessPartnerRepository
{
    Task<BusinessPartner?> GetByCodeAsync(string code, CancellationToken ct);
    Task<BusinessPartner?> GetByEmailAsync(string email, CancellationToken ct);
    System.Threading.Tasks.Task UpdateAsync(BusinessPartner partner, CancellationToken ct);

    // Dashboard & Stats methods
    Task<int> GetActiveCountAsync();
    Task<int> GetLeadsCreatedAfterAsync(DateTime date);
    Task<int> GetOpenOpportunitiesCountAsync();
    Task<int> GetOpenOpportunitiesCountAsync(string userId);
    Task<string?> GetLatestInteractionNoteAsync(string userId);
    Task<BusinessPartner?> GetTopCustomerBySalesAsync(string userId);
}
