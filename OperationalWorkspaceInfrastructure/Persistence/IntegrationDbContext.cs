using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.Interfaces.IServices;
using System.Text.Json;

// Aliases
using Attachment = OperationalWorkspace.Domain.Entities.Attachment;
using TaskEntity = OperationalWorkspace.Domain.Entities.TaskEntity;

namespace OperationalWorkspaceInfrastructure.Persistence;

public class IntegrationDbContext : DbContext
{
    private readonly IAuditLogService _auditService;
    private readonly IHttpContextAccessor _http;

    public IntegrationDbContext(
        DbContextOptions<IntegrationDbContext> options,
        IAuditLogService auditService,
        IHttpContextAccessor http)
        : base(options)
    {
        _auditService = auditService;
        _http = http;
    }

    public DbSet<BusinessPartner> BusinessPartners => Set<BusinessPartner>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<SalesOrder> SalesOrders => Set<SalesOrder>();
    public DbSet<InventoryItem> Inventories => Set<InventoryItem>();
    public DbSet<TaskEntity> Tasks => Set<TaskEntity>();
    public DbSet<AuditLogEntry> AuditLogs => Set<AuditLogEntry>();
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
        modelBuilder.Entity<TaskEntity>().HasKey(t => t.Id);
        modelBuilder.Entity<AuditLogEntry>().HasKey(a => a.Id);
        modelBuilder.Entity<Attachment>().HasKey(a => a.Id);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var auditEntries = new List<AuditLogEntry>();

        var user = _http.HttpContext?.User?.Identity?.Name ?? "System";
        var traceId = _http.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Unchanged)
                continue;

            var audit = new AuditLogEntry
            {
                Id = Guid.NewGuid(),
                UserName = user,
                TraceId = traceId,
                EntityName = entry.Entity.GetType().Name,
                OccurredAtUtc = DateTime.UtcNow,
                EventType = entry.State.ToString()
            };

            // BEFORE STATE
            if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
            {
                var before = new Dictionary<string, object?>();

                foreach (var prop in entry.OriginalValues.Properties)
                {
                    before[prop.Name] = entry.OriginalValues[prop];
                }

                audit.OldValues = JsonSerializer.Serialize(before);
            }

            // AFTER STATE
            if (entry.State == EntityState.Modified || entry.State == EntityState.Added)
            {
                var after = new Dictionary<string, object?>();

                foreach (var prop in entry.CurrentValues.Properties)
                {
                    after[prop.Name] = entry.CurrentValues[prop];
                }

                audit.NewValues = JsonSerializer.Serialize(after);
            }

            auditEntries.Add(audit);
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        if (auditEntries.Count > 0)
        {
            await _auditService.LogBulkAsync(auditEntries);
        }

        return result;
    }
}