using MediatR;
using Microsoft.EntityFrameworkCore;
using VaccinationManagement.Data;
using VaccinationManagement.Models;

namespace VaccinationManagement.Features.ScheduleFeature.Queries
{
    public class QueryGetScheduleByEmployeeDTO
    {
        public required string EmployeeId { get; set; }
        public int? PageIndex { get; set; } = 1;
        public int? PageSize { get; set; } = 10;
    }
    public class GetSchedulesByEmployeeId : IRequest<IEnumerable<Injection_Schedule>>
    {
        public required QueryGetScheduleByEmployeeDTO query { get; set; }
        public required string EmployeeId { get; set; }
  

    }

    public class GetSchedulesByEmployeeIdHandler : IRequestHandler<GetSchedulesByEmployeeId, IEnumerable<Injection_Schedule>>
    {
        private readonly ApplicationDbContext _context;

        public GetSchedulesByEmployeeIdHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Injection_Schedule>> Handle(GetSchedulesByEmployeeId request, CancellationToken cancellationToken)
        {
            var schedules = await IncludeVaccine(_context.Injection_Schedules);

            var filteredSchedules = schedules.Where(sche =>
     string.Equals(sche.CreatedByEmployee?.Id, request.EmployeeId, StringComparison.OrdinalIgnoreCase) ||
     string.Equals(sche.PerformedByEmployee?.Id, request.EmployeeId, StringComparison.OrdinalIgnoreCase)
 );


            int pageIndex = request.query.PageIndex ?? 1;
            int pageSize = request.query.PageSize ?? 10;

            int totalCount = filteredSchedules.Count();

            var items = filteredSchedules
                .OrderBy(s => s.Start_Date)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PaginatedList<Injection_Schedule>(items, totalCount, pageIndex, pageSize);

        }


        public static async Task<List<Injection_Schedule>> IncludeVaccine(DbSet<Injection_Schedule> bdset)
        {
            return await bdset
                .Include(sche => sche.Vaccine)
                .Include(sche => sche.Place)
                .Include(sche => sche.CreatedByEmployee)
                .Include(sche => sche.PerformedByEmployee)
                .Select(s => new Injection_Schedule
                {
                    Id = s.Id,
                    Description = s.Description,
                    Start_Date = s.Start_Date,
                    End_Date = s.End_Date,
                    Place = s.Place,
                    Vaccine = s.Vaccine,
                    Vaccine_Id = s.Vaccine_Id, 
                    Place_Id = s.Place_Id,
                    CreatedByEmployee = s.CreatedByEmployee,
                    PerformedByEmployee = s.PerformedByEmployee,
                })
                .ToListAsync();
        }

    }
}