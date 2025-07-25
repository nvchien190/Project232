namespace VaccinationManagement.Models.DTOs
{
    public class QueryScheduleDTO : QueryParameters
    {
        public ScheduleStatus? Status { get; set; }
        public string? VaccineId { get; set; }
        public string? VaccineName { get; set; }
        public string? PlaceName { get; set; }
        public DateOnly? ScheduleDate { get; set; } // Ngày cần tìm lịch tiêm
        public string? CreatedByEmployeeId { get; set; } // Lọc theo nhân viên tạo
        public string? PerformedByEmployeeId { get; set; } // Lọc theo nhân viên thực hiện
    }

    public class Injection_Schedules_Paged
    {
        public int CurrentPage { get; set; }

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }

        public bool HasPreviousPage { get; set; }

        public bool HasNextPage { get; set; }

    
       public virtual ICollection<Injection_Schedule>? Injection_Schedules { get; set; }

    }
}
