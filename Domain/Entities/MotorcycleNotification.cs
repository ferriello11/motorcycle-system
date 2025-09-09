namespace TesteTecnico.Domain.Entities
{
    public class MotorcycleNotification
    {
        public int Id { get; set; }
        public int MotorcycleId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
