using Microsoft.EntityFrameworkCore;
using SecureSoftwareGroupProject.Models;
using System.Collections.Generic;

namespace SecureSoftwareGroupProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
