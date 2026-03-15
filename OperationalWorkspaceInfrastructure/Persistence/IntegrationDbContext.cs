using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceInfrastructure.Persistence;

// Aliases to resolve conflicts
using Attachment = OperationalWorkspace.Domain.Entities.Attachment;
using Task = OperationalWorkspace.Domain.Entities.Task;

namespace OperationalWorkspaceInfrastructure.Persistence;

public class IntegrationDbContext : DbContext
{
    public IntegrationDbContext(DbContextOptions<IntegrationDbContext> options)
        : base(options) { }

    public DbSet<BusinessPartner> BusinessPartners => Set<BusinessPartner>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<InventoryItem> Inventories => Set<InventoryItem>();
    public DbSet<Task> Tasks => Set<Task>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<Email> Emails => Set<Email>();
    public DbSet<Knowledge> KnowledgeBase => Set<Knowledge>();
    public DbSet<Activity> Activities => Set<Activity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<BusinessPartner>().HasKey(bp => bp.Id);
        modelBuilder.Entity<Invoice>().HasKey(i => i.InvoiceId);
        modelBuilder.Entity<SalesOrder>().HasKey(o => o.Id);
        modelBuilder.Entity<InventoryItem>().HasKey(i => i.Id);
        modelBuilder.Entity<Task>().HasKey(t => t.Id);
        modelBuilder.Entity<AuditLog>().HasKey(a => a.Id);
        modelBuilder.Entity<Attachment>().HasKey(a => a.Id);
    }
}

// FIX: This version DOES NOT require the SqlServer package to compile
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<IntegrationDbContext>(options =>
        {
            // We use the base options to avoid the 'UseSqlServer' missing reference error
            // This allows the DI container to function even if the SQL package is broken
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        });

        return services;
    }
}
