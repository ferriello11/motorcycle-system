using TesteTecnico.Domain.Enums;

namespace TesteTecnico.Application.DTOs
{
    public class RegisterDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }

        public string Cnpj { get; set; }
        public string CnhNumber { get; set; }
        public CnhType CnhType { get; set; }
        public IFormFile CnhFile { get; set; }
        public DateTime DateOfBirth { get; set; }

    }
}
