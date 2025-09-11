namespace TesteTecnico.Domain.Entities
{
    public class MotorcycleNotification
    {
        public Guid Id { get; set; }
        public string Plate { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
