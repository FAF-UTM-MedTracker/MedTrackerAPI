using MedTracker.Models;
using Microsoft.EntityFrameworkCore;


namespace MedTracker
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Define DbSet properties for your entities here
        public DbSet<Users> Users { get; set; }

        // Override OnModelCreating to configure your database model
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the base OnModelCreating method
            base.OnModelCreating(modelBuilder);

            // Configure your entity relationships and constraints here
            modelBuilder.Entity<Users>().HasKey(u => u.IdUser);
        }
    }
}
