using MediatR;
using Microsoft.EntityFrameworkCore;
using VaccinationManagement.Data;
using VaccinationManagement.Models;
using VaccinationManagement.Models.DTOs;

namespace VaccinationManagement.Features.ScheduleFeature.Queries
{
    public class GetSchedulesWithPaginationQuery : IRequest<Injection_Schedules_Paged>
    {
        public required QueryScheduleDTO query { get; set; }
        public string? SearchQuery { get; set; }
    }

    public class GetSchedulesWithPagination : IRequest<Injection_Schedules_Paged>
    {
        public class GetSchedulesWithPaginationHandler : IRequestHandler<GetSchedulesWithPaginationQuery, Injection_Schedules_Paged>
        {
            private readonly ApplicationDbContext _context;
            public GetSchedulesWithPaginationHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<Injection_Schedules_Paged> Handle(GetSchedulesWithPaginationQuery request, CancellationToken cancellationToken)
            {
                
                var list = _context.Injection_Schedules
                    .Include(sche => sche.Vaccine)
                    .Include(sche => sche.Place)
                    .Include(sche => sche.CreatedByEmployee)
                    .Include(sche => sche.PerformedByEmployee)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(request.query.VaccineId))
                {
                    list = list.Where(sche => sche.Vaccine_Id == request.query.VaccineId);
                }

                if (!string.IsNullOrEmpty(request.SearchQuery))
                {
                    list = list.Where(sche =>
                        sche.Vaccine!.Vaccine_Name.Contains(request.SearchQuery) ||
                        sche.Place!.Name.Contains(request.SearchQuery)

                        );
                }
                else
                {
                    if (!string.IsNullOrEmpty(request.query.VaccineName))
                    {
                        list = list.Where(sche => sche.Vaccine!.Vaccine_Name.Contains(request.query.VaccineName));
                    }

                    if (!string.IsNullOrEmpty(request.query.PlaceName))
                    {
                        list = list.Where(sche => sche.Place!.Name.Contains(request.query.PlaceName));
                    }
                }


                if (request.query.Status != null)
                {
                    DateOnly CurrentDate = DateOnly.FromDateTime(DateTime.Now);
                    if (request.query.Status == ScheduleStatus.NotYet)
                        list = list.Where(sche => sche.Start_Date > CurrentDate);
                    else if (request.query.Status == ScheduleStatus.Over)
                        list = list.Where(sche => sche.End_Date < CurrentDate);
                    else if (request.query.Status == ScheduleStatus.Open)
                        list = list.Where(sche =>
                            sche.Start_Date <= CurrentDate &&
                            sche.End_Date >= CurrentDate
                            );
                }

                // filter by date
                if (request.query.ScheduleDate.HasValue)
                {
                    var date = request.query.ScheduleDate.Value;
                    list = list.Where(sche => sche.Start_Date <= date && sche.End_Date >= date);
                }

                // filter create emp id
                if (!string.IsNullOrEmpty(request.query.CreatedByEmployeeId))
                {
                    list = list.Where(sche => sche.CreatedByEmployeeId == request.query.CreatedByEmployeeId);
                }
                //  filter performed emp id 
                if (!string.IsNullOrEmpty(request.query.PerformedByEmployeeId))
                {
                    list = list.Where(sche => sche.PerformedByEmployeeId == request.query.PerformedByEmployeeId);
                }

                var totalEntities = list.Count();

                list = list.OrderBy(sche => sche.Id)
                    .Skip((request.query.pageIndex - 1) * request.query.PageSize)
                    .Take(request.query.PageSize);

                var entities = await list.ToListAsync(cancellationToken);

                var paginatedList = new PaginatedList<Injection_Schedule>(entities, totalEntities, request.query.pageIndex, request.query.PageSize);

                return new Injection_Schedules_Paged
                {
                    CurrentPage = paginatedList.PageIndex,
                    TotalItems = paginatedList.TotalItems,
                    TotalPages = paginatedList.TotalPages,
                    HasPreviousPage = paginatedList.HasPreviousPage,
                    HasNextPage = paginatedList.HasNextPage,
                    Injection_Schedules = paginatedList
                };
            }
        }

    }
}
