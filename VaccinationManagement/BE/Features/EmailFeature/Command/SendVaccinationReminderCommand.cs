using MediatR;

namespace VaccinationManagement.Features.EmailFeature.Command
{
    public class SendVaccinationReminderCommand : IRequest
    {
        public DateOnly Date { get; set; } // Ngày cần nhắc (thường là ngày hôm nay)
    }
} 