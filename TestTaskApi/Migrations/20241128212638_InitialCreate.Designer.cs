﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using TestTaskApi.DAL.Entities;

#nullable disable

namespace TestTaskApi.Migrations
{
    [DbContext(typeof(TestTaskContext))]
    [Migration("20241128212638_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("TestTaskApi.DAL.Entities.MedicalRecord", b =>
                {
                    b.Property<int>("MedicalRecordId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("medical_record_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int>("MedicalRecordId"));

                    b.Property<string>("Diagnosis")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("diagnosis");

                    b.Property<string>("DoctorName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("doctor_name");

                    b.Property<int>("PatientId")
                        .HasColumnType("integer")
                        .HasColumnName("patient_id");

                    b.Property<string>("Recomendations")
                        .HasColumnType("text")
                        .HasColumnName("recomendations");

                    b.Property<DateTime>("RecordDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("record_date");

                    b.HasKey("MedicalRecordId")
                        .HasName("MedicalRecord_pkey");

                    b.HasIndex("PatientId");

                    b.ToTable("MedicalRecord", (string)null);
                });

            modelBuilder.Entity("TestTaskApi.DAL.Entities.Patient", b =>
                {
                    b.Property<int>("PatientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("patient_id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityAlwaysColumn(b.Property<int>("PatientId"));

                    b.Property<DateTime>("BirthDate")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("birth_date");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("first_name");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("last_name");

                    b.Property<string>("PolisNumber")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("polis_number");

                    b.HasKey("PatientId")
                        .HasName("Patient_pkey");

                    b.HasIndex(new[] { "PolisNumber" }, "Polis number unique")
                        .IsUnique();

                    b.ToTable("Patient", (string)null);
                });

            modelBuilder.Entity("TestTaskApi.DAL.Entities.MedicalRecord", b =>
                {
                    b.HasOne("TestTaskApi.DAL.Entities.Patient", "Patient")
                        .WithMany("MedicalRecords")
                        .HasForeignKey("PatientId")
                        .IsRequired()
                        .HasConstraintName("MedicalRecord_patient_id_fkey");

                    b.Navigation("Patient");
                });

            modelBuilder.Entity("TestTaskApi.DAL.Entities.Patient", b =>
                {
                    b.Navigation("MedicalRecords");
                });
#pragma warning restore 612, 618
        }
    }
}
