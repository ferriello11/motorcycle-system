using System.ComponentModel.DataAnnotations;
using TesteTecnico.Domain.Enums;

namespace TesteTecnico.Domain.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public string Role { get; set; }

        [MaxLength(20)]
        public string Cnpj { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [MaxLength(20)]
        public string CnhNumber { get; set; }

        [MaxLength(3)]
        public CnhType CnhType { get; set; } // "A", "B" ou "A+B"

        [MaxLength(200)]
        public string CnhImageUrl { get; set; }
    }
}
