using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.Interfaces.IRepository;
using Microsoft.EntityFrameworkCore;
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
}