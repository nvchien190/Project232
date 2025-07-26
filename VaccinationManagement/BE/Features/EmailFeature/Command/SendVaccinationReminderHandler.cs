using MediatR;
using Microsoft.EntityFrameworkCore;
using VaccinationManagement.Data;
using VaccinationManagement.Models;

namespace VaccinationManagement.Features.EmailFeature.Command
{
    public class SendVaccinationReminderHandler : IRequestHandler<SendVaccinationReminderCommand>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;

        public SendVaccinationReminderHandler(ApplicationDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(SendVaccinationReminderCommand request, CancellationToken cancellationToken)
        {
            var remindDate = request.Date.AddDays(1); // Lấy khách hàng có lịch tiêm vào ngày mai

            var customers = await _context.Injection_Results
                .Include(r => r.Customer)
                .Where(r => r.Next_Injection_Date == remindDate && r.Customer != null && !string.IsNullOrEmpty(r.Customer.Email))
                .Select(r => r.Customer)
                .Distinct()
                .ToListAsync(cancellationToken);

            foreach (var customer in customers)
            {
                await _mediator.Send(new SendEmailCommand
                {
                    To = customer.Email!,
                    Subject = "Nhắc lịch tiêm chủng",
                    Body = $"Xin chào {customer.Full_Name},\nBạn đã đến lịch tiêm chủng vào ngày {remindDate:dd/MM/yyyy}. Vui lòng đến đúng hẹn!"
                });
            }

            return Unit.Value;
        }
    }
} 