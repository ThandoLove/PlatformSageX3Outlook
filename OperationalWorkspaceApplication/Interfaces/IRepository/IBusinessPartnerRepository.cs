using OperationalWorkspace.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OperationalWorkspaceApplication.Interfaces.IRepository;

public interface IBusinessPartnerRepository
{
    Task<BusinessPartner?> GetByCodeAsync(string code, CancellationToken ct);

    // IMPORTANT: this is what your builder uses
    Task<BusinessPartner?> GetByEmailAsync(string email, CancellationToken ct);

    Task UpdateAsync(BusinessPartner partner, CancellationToken ct);

    Task<int> GetActiveCountAsync();
    Task<int> GetLeadsCreatedAfterAsync(DateTime date);
    Task<int> GetOpenOpportunitiesCountAsync();
    Task<int> GetOpenOpportunitiesCountAsync(string userId);
    Task<string?> GetLatestInteractionNoteAsync(string userId);
    Task<BusinessPartner?> GetTopCustomerBySalesAsync(string userId);
}