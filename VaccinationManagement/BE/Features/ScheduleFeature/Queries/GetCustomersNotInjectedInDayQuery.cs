using MediatR;
using Microsoft.EntityFrameworkCore;
using VaccinationManagement.Data;
using VaccinationManagement.Models;

namespace VaccinationManagement.Features.ScheduleFeature.Queries
{
    public class GetCustomersNotInjectedInDayQuery : IRequest<List<Customer>>
    {
        public required DateOnly Date { get; set; }
    }

    public class GetCustomersNotInjectedInDayHandler : IRequestHandler<GetCustomersNotInjectedInDayQuery, List<Customer>>
    {
        private readonly ApplicationDbContext _context;
        public GetCustomersNotInjectedInDayHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> Handle(GetCustomersNotInjectedInDayQuery request, CancellationToken cancellationToken)
        {
            // Lấy tất cả khách hàng
            var allCustomers = _context.Customers.Where(c => c.Status == true);

            // Lấy danh sách khách hàng đã tiêm trong ngày
            var injectedCustomerIds = await _context.Injection_Results
                .Where(r => r.Injection_Date == request.Date && r.IsVaccinated == ResultStatus.Injected)
                .Select(r => r.Customer_Id)
                .Distinct()
                .ToListAsync(cancellationToken);

            // Lọc ra khách hàng chưa tiêm trong ngày
            var notInjectedCustomers = await allCustomers
                .Where(c => !injectedCustomerIds.Contains(c.Id))
                .ToListAsync(cancellationToken);

            return notInjectedCustomers;
        }
    }
} 