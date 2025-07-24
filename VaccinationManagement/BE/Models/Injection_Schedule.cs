using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VaccinationManagement.Models
{
    public enum ScheduleStatus
    {
        NotYet,
        Open,
        Over,
    }

    public class Injection_Schedule : BaseEntity
    {
        [MaxLength(1000)]
        public string? Description { get; set; }

        public required DateOnly End_Date { get; set; }

        [ForeignKey("Place")]
        public required string Place_Id { get; set; }

        public required DateOnly Start_Date { get; set; }

        [ForeignKey("Vaccine")]
        public required string Vaccine_Id { get; set; }

        public virtual Vaccine? Vaccine { get; set; }

        public virtual Place? Place { get; set; }

        [JsonIgnore]
        public string? PerformedByEmployeeId { get; set; }
        [JsonIgnore]
        public string? CreatedByEmployeeId { get; set; } // Người tạo lịch
        [ForeignKey("CreatedByEmployeeId")]
        public virtual Employee? CreatedByEmployee { get; set; }

     // Người thực hiện tiêm
        [ForeignKey("PerformedByEmployeeId")]
        public virtual Employee? PerformedByEmployee { get; set; }

    }
}
