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
        public DbSet<Patient_Treatment> Patient_Treatment { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<Treatment_Medication> Treatment_Medication { get; set; }
        //public DbSet<NotesP> NotesPs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the base OnModelCreating method
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>()
                .HasKey(u => u.IdUser);

            // Configure your entity relationships and constraints here
            modelBuilder.Entity<User>().HasKey(u => u.IdUser);
            modelBuilder.Entity<User>()
                .HasMany(t => t.Treatments);

            modelBuilder.Entity<Doctor>()
                .HasKey(u => u.IdUser);
            modelBuilder.Entity<Doctor>()
                .HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.IdUser);

            modelBuilder.Entity<Patient>()
                .HasKey(u => u.IdUser);
            modelBuilder.Entity<Patient>()
                .HasOne(p => p.User)
                .WithOne(u => u.Patient)
                .HasForeignKey<Patient>(d => d.IdUser);

            // Treatment
            modelBuilder.Entity<Treatment>()
                .HasKey(t => t.IdTreatment);
            modelBuilder.Entity<Treatment>()
            .Property(t => t.TName);
            modelBuilder.Entity<Treatment>()
            .Property(t => t.NoteDoctor);
            // corect
            modelBuilder.Entity<Treatment>()
                .HasOne(user => user.Doctor)
                .WithMany(t => t.Treatments)
                .HasForeignKey(u => u.DoctorID)
                .IsRequired();

            // Pacient_Treatment For User
            modelBuilder.Entity<Patient_Treatment>()
                .HasKey(pt => pt.IdTreatment);
            //modelBuilder.Entity<Patient_Treatment>()
            //    .HasKey(pt => new{ pt.IdUser,pt.IdTreatment});

            // gata
            modelBuilder.Entity<Patient_Treatment>()
                .HasOne(pt => pt.Patient)
                .WithMany(u => u.Patient_Treatment)
                .HasForeignKey(pt => pt.IdUser);

            // Pacient_Treatment For treatment
            //modelBuilder.Entity<Patient_Treatment>()
            //  .HasKey(pt => pt.IdTreatment);

            // gata
            modelBuilder.Entity<Patient_Treatment>()
                .HasOne(pt => pt.Treatment)
                .WithMany(tr => tr.Patient_Treatment)
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
