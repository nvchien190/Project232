using MediatR;
using VaccinationManagement.Data;
using VaccinationManagement.Models;

namespace VaccinationManagement.Features.VaccineFeature.Commands
{
    public class ReduceVaccineQuantity : IRequest<string>
    {
        public required string Vaccine_Id { get; set; }
        public required int Quantity { get; set; }

        public class ReduceVaccineQuantityHandler : IRequestHandler<ReduceVaccineQuantity, string>
        {
            private readonly ApplicationDbContext _context;

            public ReduceVaccineQuantityHandler(ApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<string> Handle(ReduceVaccineQuantity command, CancellationToken cancellationToken)
            {
                var vaccine = await _context.Vaccines.FindAsync(command.Vaccine_Id);
                if (vaccine == null)
                {
                    return "Vaccine not found";
                }
                if (vaccine.Number_Of_Injection == null || vaccine.Number_Of_Injection < command.Quantity)
                {
                    return "Insufficient vaccine stock";
                }
                vaccine.Number_Of_Injection -= command.Quantity;
                _context.Vaccines.Update(vaccine);
                await _context.SaveChangesAsync(cancellationToken);
                return "Vaccine quantity reduced successfully";
            }
        }
    }
} 