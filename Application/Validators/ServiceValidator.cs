using Microsoft.EntityFrameworkCore;
using TesteTecnico.Domain.Entities;
using TesteTecnico.Domain.Enums;
using TesteTecnico.Infrastructure.Data;

namespace TesteTecnico.Application.Validators
{
    public static class ServiceValidator
    {
        public static async Task<User> ValidateUserExists(AppDbContext context, Guid userId)
        {
            var user = await context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("Usuário não encontrado.");
            return user;
        }

        public static async Task ValidateUniqueEmail(AppDbContext context, string email)
        {
            if (await context.Users.AnyAsync(u => u.Email == email))
                throw new Exception("Email já cadastrado.");
        }

        public static async Task ValidateUniqueCnh(AppDbContext context, string cnhNumber)
        {
            if (await context.Users.AnyAsync(u => u.CnhNumber == cnhNumber))
                throw new Exception("CNH já cadastrada.");
        }

        public static async Task ValidateUniqueCnpj(AppDbContext context, string cnpj)
        {
            if (await context.Users.AnyAsync(u => u.Cnpj == cnpj))
                throw new Exception("CNPJ já cadastrado.");
        }


        public static async Task ValidateUniqueMotorcyclePlateAsync(AppDbContext context, string plate)
        {
            if (await context.Motorcycles.AnyAsync(m => m.Plate == plate))
                throw new Exception("Placa já cadastrada");
        }

        public static async Task<Motorcycle> ValidateMotorcycleExists(AppDbContext context, string plate)
        {
            var motorcycle = await context.Motorcycles.FirstOrDefaultAsync(m => m.Plate == plate);
            if (motorcycle == null)
                throw new Exception("Moto não encontrada.");
            return motorcycle;
        }

        public static void ValidateDelivererRole(User user)
        {
            if (user.Role != UserRole.Deliverer.ToString())
                throw new Exception("Somente entregadores podem alugar motos.");
        }

        public static void ValidateDelivererCnh(User user)
        {
            if (user.CnhType != CnhType.A)
                throw new Exception("Somente entregadores com CNH categoria A podem alugar motos.");
        }

    }
}
