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
        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }

        // Override OnModelCreating to configure your database model
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the base OnModelCreating method
            base.OnModelCreating(modelBuilder);

            // Configure your entity relationships and constraints here
            modelBuilder.Entity<User>().HasKey(u => u.IdUser);
            modelBuilder.Entity<Doctor>()
                .HasKey(u => u.IdUser);
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d=>d.IdUser);
            
            modelBuilder.Entity<Patient>()
                .HasKey(u => u.IdUser);
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithOne(u => u.Patient)
                .HasForeignKey<Patient>(d => d.IdUser);


        }
    }
}
