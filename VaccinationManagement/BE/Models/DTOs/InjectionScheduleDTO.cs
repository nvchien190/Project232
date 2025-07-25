namespace VaccinationManagement.Models.DTOs
{
    public class EmployeeBasicDTO
    {
        public string Id { get; set; }
        public string Employee_Name { get; set; }
    }

    public class InjectionScheduleDTO
    {
        public string Id { get; set; }
        public string? Description { get; set; }
        public DateOnly End_Date { get; set; }
        public string Place_Id { get; set; }
        public DateOnly Start_Date { get; set; }
        public string Vaccine_Id { get; set; }
        public Vaccine? Vaccine { get; set; }
        public Place? Place { get; set; }
        public EmployeeBasicDTO? CreatedByEmployee { get; set; }
        public EmployeeBasicDTO? PerformedByEmployee { get; set; }
    }
} 