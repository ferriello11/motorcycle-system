using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TesteTecnico.Domain.Entities
{
    public class Rental
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Required]
        public Guid MotorcycleId { get; set; }
        [ForeignKey(nameof(MotorcycleId))]
        public Motorcycle Motorcycle { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime ExpectedEndDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public decimal TotalCost { get; set; }
    }
}
