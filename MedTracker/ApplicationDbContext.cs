using MedTracker.Models;
using Microsoft.EntityFrameworkCore;


namespace MedTracker
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<Patient_Treatment> Patient_Treatments { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<Treatment_Medication> Treatment_Medications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the base OnModelCreating method
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>()
                .HasKey(u => u.IdUser);
            
            // Patient
            modelBuilder.Entity<Patient>()
                .HasKey(p => p.IdUser);

            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithMany(p => p.Patients)
                .HasForeignKey(p => p.IdUser);

            // Doctor
            modelBuilder.Entity<Doctor>()
                .HasKey(d => d.IdUser);

            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithMany(d => d.Doctors)
                .HasForeignKey(d => d.IdUser);

            // Treatment
            modelBuilder.Entity<Treatment>()
                .HasKey(t => t.IdTreatment);

            // corect
            modelBuilder.Entity<Treatment>()
                .HasOne(user => user.User)
                .WithMany(t => t.Treatments)
                .HasForeignKey(u => u.IdUserD);

            // Pacient_Treatment For User
            modelBuilder.Entity<Patient_Treatment>()
                .HasKey(pt => pt.IdTreatment);
            //modelBuilder.Entity<Patient_Treatment>()
            //    .HasKey(pt => new{ pt.IdUser,pt.IdTreatment});

            // gata
            modelBuilder.Entity<Patient_Treatment>()
                .HasOne(pt => pt.User)
                .WithMany(u => u.Patient_Treatments)
                .HasForeignKey(pt => pt.IdUser);

            // Pacient_Treatment For treatment
            //modelBuilder.Entity<Patient_Treatment>()
            //  .HasKey(pt => pt.IdTreatment);

            // gata
            modelBuilder.Entity<Patient_Treatment>()
                .HasOne(pt => pt.Treatment)
                .WithMany(tr => tr.Patient_Treatments)
                .HasForeignKey(pt => pt.IdTreatment);

            // Medication
            //modelBuilder.Entity<Medication>()
            //    .HasKey(m => m.IdMedication);

            // Treatment_Medication for Treatment
            modelBuilder.Entity<Treatment_Medication>()
                .HasKey(tm => tm.IdTreatment);

            modelBuilder.Entity<Treatment_Medication>()
                .HasOne(tm => tm.Treatment)
                .WithMany(treatment => treatment.Treatment_Medications)
                .HasForeignKey(tm => tm.IdTreatment);

            // Treatment_Medication for Medication
            modelBuilder.Entity<Treatment_Medication>()
                .HasKey(tm => tm.IdMedication);

            modelBuilder.Entity<Treatment_Medication>()
                .HasOne(tm => tm.Medication)
                .WithMany(med => med.Treatment_Medications)
                .HasForeignKey(tm => tm.IdMedication);
        }
    }
}
