using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TestTaskApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Patient",
                columns: table => new
                {
                    patient_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    birth_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    polis_number = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Patient_pkey", x => x.patient_id);
                });

            migrationBuilder.CreateTable(
                name: "MedicalRecord",
                columns: table => new
                {
                    medical_record_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    record_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    diagnosis = table.Column<string>(type: "text", nullable: false),
                    doctor_name = table.Column<string>(type: "text", nullable: false),
                    recomendations = table.Column<string>(type: "text", nullable: true),
                    patient_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("MedicalRecord_pkey", x => x.medical_record_id);
                    table.ForeignKey(
                        name: "MedicalRecord_patient_id_fkey",
                        column: x => x.patient_id,
                        principalTable: "Patient",
                        principalColumn: "patient_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecord_patient_id",
                table: "MedicalRecord",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "Polis number unique",
                table: "Patient",
                column: "polis_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MedicalRecord");

            migrationBuilder.DropTable(
                name: "Patient");
        }
    }
}
