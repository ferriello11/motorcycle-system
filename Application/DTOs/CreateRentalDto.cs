using TesteTecnico.Domain.Enums;

namespace TesteTecnico.Application.DTOs
{
    public class CreateRentalDto
    {
        public string Plate { get; set; }
        public RentalPlan Plan { get; set; }
    }
}
