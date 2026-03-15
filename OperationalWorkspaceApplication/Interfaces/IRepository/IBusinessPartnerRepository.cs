using OperationalWorkspace.Domain.Entities;

namespace OperationalWorkspaceApplication.Interfaces.IRepository;

public interface IBusinessPartnerRepository
{
    Task<BusinessPartner?> GetByCodeAsync(string code, CancellationToken ct);
    System.Threading.Tasks.Task UpdateAsync(BusinessPartner partner, CancellationToken ct);
}