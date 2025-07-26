using MediatR;
using Microsoft.EntityFrameworkCore;
using VaccinationManagement.Data;

namespace VaccinationManagement.Features.CustomerFeature.Queries
{
	public class AnnualVaccineResponse
	{
        public required string VaccineName { get; set; }
        public required int TotalInject{ get; set; }
		public required int TotalInjectionVisits { get; set; }
    }

	public class GetCustomerAnnualVaccineQuery : IRequest<List<AnnualVaccineResponse>>
	{
		public required int ?Year { get; set; }
		public required string CustomerId { get; set; }

		public class GetCustomerAnnualVaccine : IRequestHandler<GetCustomerAnnualVaccineQuery, List<AnnualVaccineResponse>>
		{
			private readonly ApplicationDbContext _context;
            public GetCustomerAnnualVaccine(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<List<AnnualVaccineResponse>> Handle(GetCustomerAnnualVaccineQuery request, CancellationToken cancellationToken)
			{
                int? year = request.Year;

                // filter by year
                var query = _context.Injection_Results.Where(i => i.Customer_Id == request.CustomerId && i.Injection_Date.HasValue);

                if (year.HasValue)
                {
                    query = query.Where(i => i.Injection_Date!.Value.Year == year.Value);
                }

                var totalInjectionVisits = await query.CountAsync(cancellationToken);

                var result = await query
                    .GroupBy(i => i.Vaccine!.Vaccine_Name)
                    .Select(g => new AnnualVaccineResponse
                    {
                        VaccineName = g.Key,
                        TotalInject = g.Sum(i => i.Number_Of_Injection),
                        TotalInjectionVisits = totalInjectionVisits
                    })
                    .ToListAsync(cancellationToken);
                return result;

            }
        }
	}
}
