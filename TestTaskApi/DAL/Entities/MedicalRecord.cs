using System;
using System.Collections.Generic;

namespace TestTaskApi.DAL.Entities;

public partial class MedicalRecord
{
    public int MedicalRecordId { get; set; }

    public DateTime RecordDate { get; set; }

    public string Diagnosis { get; set; } = null!;

    public string DoctorName { get; set; } = null!;

    public string? Recomendations { get; set; }

    public int PatientId { get; set; }

    public virtual Patient Patient { get; set; } = null!;
}
