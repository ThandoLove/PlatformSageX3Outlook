
using Microsoft.EntityFrameworkCore;
using OperationalWorkspace.Domain.Entities;
using System;

namespace OperationalWorkspaceInfrastructure.Persistence;

public class PlatformDbContext : DbContext
{
    public PlatformDbContext(DbContextOptions<PlatformDbContext> options) : base(options)
    {
    }

    // ✅ REGISTERED APPLICABLE ENTITIES
    public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
    public DbSet<BusinessPartner> BusinessPartners => Set<BusinessPartner>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed sample test operators to populate your dynamic Blazor user grids automatically
        var adminId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var operatorId = Guid.NewGuid();

        modelBuilder.Entity<UserAccount>().HasData(
            new UserAccount { Id = adminId, Username = "System Administrator", Email = "admin@sagex3.com", Role = "Admin", IsActive = true, CreatedAt = DateTime.UtcNow },
            new UserAccount { Id = managerId, Username = "Operations Manager", Email = "manager@sagex3.com", Role = "Manager", IsActive = true, CreatedAt = DateTime.UtcNow },
            new UserAccount { Id = operatorId, Username = "Normal Operator", Email = "operator@test.com", Role = "User", IsActive = true, CreatedAt = DateTime.UtcNow }
        );

        // Seed sample customer metrics to drive dashboard analytics pipelines
        modelBuilder.Entity<BusinessPartner>().HasData(
            new { Id = 1, BpCode = "SAGE-BP-001", Company = "X3 Consulting Corp", CreditLimit = 50000m, OverdueInvoices = 0m, LastContactDate = DateTime.UtcNow, IsActive = true, IsLead = false, CreatedAt = DateTime.UtcNow, HasOpenOpportunity = true, AssignedToUserId = operatorId.ToString(), TotalSalesVolume = 125000m },
            new { Id = 2, BpCode = "SAGE-BP-002", Company = "Logistics Enterprise Ltd", CreditLimit = 25000m, OverdueInvoices = 4500m, LastContactDate = DateTime.UtcNow, IsActive = true, IsLead = true, CreatedAt = DateTime.UtcNow, HasOpenOpportunity = false, AssignedToUserId = operatorId.ToString(), TotalSalesVolume = 42000m }
        );
    }
}
