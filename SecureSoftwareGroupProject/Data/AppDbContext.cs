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

            modelBuilder.Entity<CustomerBalance>().ToTable("CustomerBalance");
            modelBuilder.Entity<ProviderProfile>().ToTable("ProviderProfile");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<CustomerReview>().ToTable("CustomerReview");

            modelBuilder.Entity<CustomerBalance>().Property(p => p.Balance).HasPrecision(18, 2);
            modelBuilder.Entity<CustomerBalance>().Property(p => p.CreditLimit).HasPrecision(18, 2);
            modelBuilder.Entity<ProviderProfile>().Property(p => p.HourlyRateAmount).HasPrecision(18, 2);
            modelBuilder.Entity<ProviderProfile>().Property(p => p.CalloutFeeAmount).HasPrecision(18, 2);
        }
    }
}
