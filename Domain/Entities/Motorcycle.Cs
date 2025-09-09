namespace TesteTecnico.Domain.Entities
{
    public class Motorcycle
    {
        public int Id { get; set; }
        public string Identifier { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string Plate { get; set; } = null!;
        public int Year { get; set; }
    }
}
