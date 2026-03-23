using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using Microsoft.EntityFrameworkCore;
// Alias to resolve conflict with Domain.Entities.Task
using Task = System.Threading.Tasks.Task;

namespace OperationalWorkspaceInfrastructure.Persistence.Repositories;

public class BusinessPartnerRepository : IBusinessPartnerRepository
{
    private readonly IntegrationDbContext _db;

    public BusinessPartnerRepository(IntegrationDbContext db) => _db = db;

    public async Task<BusinessPartner?> GetByCodeAsync(string bpCode, CancellationToken cancellationToken)
        => await _db.BusinessPartners.FirstOrDefaultAsync(bp => bp.BpCode == bpCode, cancellationToken);

    public async Task UpdateAsync(BusinessPartner partner, CancellationToken cancellationToken)
    {
        _db.BusinessPartners.Update(partner);
        await _db.SaveChangesAsync(cancellationToken);
    }

    // ============================================================
    // FIX: Implementation of Dashboard & Stats methods
    // ============================================================

    public async Task<int> GetActiveCountAsync()
    {
        // Assuming your BusinessPartner entity has an 'IsActive' property
        return await _db.BusinessPartners.CountAsync(bp => bp.IsActive);
    }

    public async Task<int> GetLeadsCreatedAfterAsync(DateTime date)
    {
        // Assuming 'IsLead' and 'CreatedAt' properties exist
        return await _db.BusinessPartners
            .CountAsync(bp => bp.IsLead && bp.CreatedAt >= date);
    }

    public async Task<int> GetOpenOpportunitiesCountAsync()
    {
        // Global count of open opportunities
        return await _db.BusinessPartners.CountAsync(bp => bp.HasOpenOpportunity);
    }

    public async Task<int> GetOpenOpportunitiesCountAsync(string userId)
    {
        // User-specific count of open opportunities
        return await _db.BusinessPartners
            .CountAsync(bp => bp.HasOpenOpportunity && bp.AssignedToUserId == userId);
    }

    public async Task<string?> GetLatestInteractionNoteAsync(string userId)
    {
        // Returns the most recent interaction note for a user's partners
        return await _db.BusinessPartners
            .Where(bp => bp.AssignedToUserId == userId && bp.LastInteractionNote != null)
            .OrderByDescending(bp => bp.LastInteractionDate)
            .Select(bp => bp.LastInteractionNote)
            .FirstOrDefaultAsync();
    }

    public async Task<BusinessPartner?> GetTopCustomerBySalesAsync(string userId)
    {
        // Returns the customer with the highest sales volume assigned to this user
        return await _db.BusinessPartners
            .Where(bp => bp.AssignedToUserId == userId)
            .OrderByDescending(bp => bp.TotalSalesVolume)
            .FirstOrDefaultAsync();
    }
}
