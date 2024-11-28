using System.ComponentModel.DataAnnotations;

namespace TestTaskApi.DAL.Entities
{
    public class PatientDTO
    {
        public int PatientId { get; set; }
        [Required(ErrorMessage ="Поле FirstName не должно быть пустым")]
        public string FirstName { get; set; } = null!;
        [Required(ErrorMessage = "Поле LastName не должно быть пустым")]
        public string LastName { get; set; } = null!;
        [Required(ErrorMessage = "Поле BirthDate не должно быть пустым")]
        public string BirthDate { get; set; } = null!;
        [Required(ErrorMessage = "Поле PolisNumber не должно быть пустым")]
        public string PolisNumber { get; set; } = null!;

    }
}
