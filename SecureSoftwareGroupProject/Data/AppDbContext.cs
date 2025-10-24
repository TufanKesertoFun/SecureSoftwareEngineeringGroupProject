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
        public DbSet<CustomerReview> CustomerReviews => Set<CustomerReview>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Default schema
            modelBuilder.HasDefaultSchema("dbo");

            // Table mappings (respecting your class names, just pinning to dbo)
            modelBuilder.Entity<CustomerBalance>().ToTable("CustomerBalance", "dbo");
            modelBuilder.Entity<ProviderProfile>().ToTable("ProviderProfile", "dbo");
            modelBuilder.Entity<User>().ToTable("Users", "dbo");
            modelBuilder.Entity<CustomerReview>().ToTable("CustomerReview", "dbo");

            // Decimal precision for currency-like fields (no entity changes)
            modelBuilder.Entity<CustomerBalance>().Property(p => p.Balance).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<CustomerBalance>().Property(p => p.CreditLimit).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ProviderProfile>().Property(p => p.HourlyRateAmount).HasColumnType("decimal(18,2)");
            modelBuilder.Entity<ProviderProfile>().Property(p => p.CalloutFeeAmount).HasColumnType("decimal(18,2)");

            // No relationships or extra indexes configured since you didn't define nav props
        }
    }
}
