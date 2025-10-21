using Microsoft.EntityFrameworkCore;
using SecureSoftwareGroupProject.Models; // CustomerBalance, User
using SecureSoftwareGroupProject.Model;  // ProviderProfile (note singular namespace)

namespace SecureSoftwareGroupProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<CustomerBalance> CustomerBalances => Set<CustomerBalance>();
        public DbSet<ProviderProfile> ProviderProfiles => Set<ProviderProfile>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            base.OnModelCreating(mb);

            // Default schema
            mb.HasDefaultSchema("dbo");

            // Table mappings (respecting your class names, just pinning to dbo)
            mb.Entity<CustomerBalance>().ToTable("CustomerBalance", "dbo");
            mb.Entity<ProviderProfile>().ToTable("ProviderProfile", "dbo");
            mb.Entity<User>().ToTable("Users", "dbo");

            // Decimal precision for currency-like fields (no entity changes)
            mb.Entity<CustomerBalance>().Property(p => p.Balance).HasColumnType("decimal(18,2)");
            mb.Entity<CustomerBalance>().Property(p => p.CreditLimit).HasColumnType("decimal(18,2)");
            mb.Entity<ProviderProfile>().Property(p => p.HourlyRateAmount).HasColumnType("decimal(18,2)");
            mb.Entity<ProviderProfile>().Property(p => p.CalloutFeeAmount).HasColumnType("decimal(18,2)");

            // No relationships or extra indexes configured since you didn't define nav props
        }
    }
}
