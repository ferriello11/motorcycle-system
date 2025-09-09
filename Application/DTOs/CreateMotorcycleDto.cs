namespace TesteTecnico.Application.DTOs
{
    public class CreateMotorcycleDto
    {
        public string Identifier { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string Plate { get; set; } = null!;
        public int Year { get; set; }
    }
}
