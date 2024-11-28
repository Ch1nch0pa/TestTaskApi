using System.ComponentModel.DataAnnotations;

namespace TestTaskApi.DAL.Entities
{
    public class MedicalRecordDto
    {
        public int MedicalRecordId { get; set; }
        [Required(ErrorMessage = "Поле RecordDate не должно быть пустым")]
        public string RecordDate { get; set; } = null!;
        [Required(ErrorMessage = "Поле Diagnosis не должно быть пустым")]
        public string Diagnosis { get; set; } = null!;
        [Required(ErrorMessage = "Поле DoctorName не должно быть пустым")]
        public string DoctorName { get; set; } = null!;

        public string? Recomendations { get; set; }
        [Required(ErrorMessage = "Поле PatientId  не должно быть пустым")]
        public int PatientId { get; set; }
    }
}
