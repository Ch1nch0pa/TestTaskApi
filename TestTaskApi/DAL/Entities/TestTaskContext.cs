using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TestTaskApi.DAL.Entities;

public partial class TestTaskContext : DbContext
{
    public TestTaskContext()
    {
    }

    public TestTaskContext(DbContextOptions<TestTaskContext> options)
        : base(options)
    {
    }

    public virtual DbSet<MedicalRecord> MedicalRecords { get; set; }

    public virtual DbSet<Patient> Patients { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Name=ConnectionStrings:DefaultConnection");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MedicalRecord>(entity =>
        {
            entity.HasKey(e => e.MedicalRecordId).HasName("MedicalRecord_pkey");

            entity.ToTable("MedicalRecord");

            entity.Property(e => e.MedicalRecordId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("medical_record_id");
            entity.Property(e => e.Diagnosis).HasColumnName("diagnosis");
            entity.Property(e => e.DoctorName).HasColumnName("doctor_name");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.Recomendations).HasColumnName("recomendations");
            entity.Property(e => e.RecordDate).HasColumnName("record_date");

            entity.HasOne(d => d.Patient).WithMany(p => p.MedicalRecords)
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("MedicalRecord_patient_id_fkey");
        });

        modelBuilder.Entity<Patient>(entity =>
        {
            entity.HasKey(e => e.PatientId).HasName("Patient_pkey");

            entity.ToTable("Patient");

            entity.HasIndex(e => e.PolisNumber, "Polis number unique").IsUnique();

            entity.Property(e => e.PatientId)
                .UseIdentityAlwaysColumn()
                .HasColumnName("patient_id");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.FirstName).HasColumnName("first_name");
            entity.Property(e => e.LastName).HasColumnName("last_name");
            entity.Property(e => e.PolisNumber).HasColumnName("polis_number");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
