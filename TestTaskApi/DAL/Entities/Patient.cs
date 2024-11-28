using System;
using System.Collections.Generic;

namespace TestTaskApi.DAL.Entities;

public partial class Patient
{
    public int PatientId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime BirthDate { get; set; }

    public string PolisNumber { get; set; } = null!;

    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
}
