using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OperationalWorkspace.Domain.Entities;
using OperationalWorkspaceApplication.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

// Aliases (All Relevant Project Original Aliases Kept 100% Intact)
using Attachment = OperationalWorkspace.Domain.Entities.Attachment;
using TaskEntity = OperationalWorkspace.Domain.Entities.TaskEntity;

namespace OperationalWorkspaceInfrastructure.Persistence;

public class IntegrationDbContext : DbContext
{
    private readonly IAuditLogService _auditService;
    private readonly IHttpContextAccessor _http;
    private readonly IBackgroundTaskQueue _backgroundQueue; // 🚀 NEW: Integrated background thread pipeline

    public IntegrationDbContext(
        DbContextOptions<IntegrationDbContext> options,
        IAuditLogService auditService,
        IHttpContextAccessor http,
        IBackgroundTaskQueue backgroundQueue) // Injected from your background processing architecture
        : base(options)
    {
        _auditService = auditService;
        _http = http;
        _backgroundQueue = backgroundQueue;
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

        // Core Primary Keys Mappings (100% Preserved)
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
        // 🚀 CRITICAL ESCAPE HATCH: 
        // If the context is currently processing internal AuditLog entries, skip generating secondary audits.
        // This stops infinite deadlocks, database connection depletion, and application crash loops completely.
        var containsOnlyAudits = ChangeTracker.Entries().All(e => e.Entity is AuditLogEntry);
        if (containsOnlyAudits)
        {
            return await base.SaveChangesAsync(cancellationToken);
        }

        var auditEntries = new List<AuditLogEntry>();

        var user = _http.HttpContext?.User?.Identity?.Name ?? "System";
        var traceId = _http.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Unchanged || entry.Entity is AuditLogEntry)
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

            // BEFORE STATE (100% Original Dictionary Logic Preserved)
            if (entry.State == EntityState.Modified || entry.State == EntityState.Deleted)
            {
                var before = new Dictionary<string, object?>();

                foreach (var prop in entry.OriginalValues.Properties)
                {
                    before[prop.Name] = entry.OriginalValues[prop];
                }

                audit.OldValues = JsonSerializer.Serialize(before);
            }

            // AFTER STATE (100% Original Dictionary Logic Preserved)
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

        // Commit the original user business changes to the SQL Server first
        var result = await base.SaveChangesAsync(cancellationToken);

        // 🚀 STABILIZED BACKGROUND AUDITING:
        // Offloads bulk log generation to your thread-safe channel queue. The main thread returns instantly, 
        // keeping the Outlook UI completely fluid and lag-free while processing audits in the background safely.
        if (auditEntries.Count > 0)
        {
            await _backgroundQueue.QueueBackgroundWorkItemAsync(async token =>
            {
                await _auditService.LogBulkAsync(auditEntries);
            });
        }

        return result;
    }
}
