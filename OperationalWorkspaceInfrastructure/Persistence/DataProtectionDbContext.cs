
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OperationalWorkspaceInfrastructure.Persistence
{
    public class DataProtectionDbContext : DbContext, IDataProtectionKeyContext
    {
        public DataProtectionDbContext(DbContextOptions<DataProtectionDbContext> options) : base(options) { }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<DataProtectionKey>(entity =>
            {
                entity.ToTable("DataProtectionKeys", "security");
                entity.HasKey(e => e.Id);
            });
        }
    }
}
