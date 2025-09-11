using TesteTecnico.Application.DTOs;
using TesteTecnico.Application.Interfaces;
using TesteTecnico.Domain.Entities;
using TesteTecnico.Domain.Enums;
using TesteTecnico.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TesteTecnico.Application.Validators;

namespace TesteTecnico.Application.Services
{
    public class RentalService : IRentalService
    {
        private readonly AppDbContext _context;

        public RentalService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Rental> CreateRentalAsync(Guid userId, CreateRentalDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            await ServiceValidator.ValidateUserExists(_context, userId);
            //TODO AJUSTAR PARA RETIRAR AS VALIDAÇÕES E UTILIZAR ORQUESTRADOR PARA ISSO
            
            if (user.Role != "Deliverer")
                throw new Exception("Somente entregadores podem alugar motos.");

            if (user.CnhType != CnhType.A)
                throw new Exception("Somente entregadores com CNH categoria A podem alugar motos.");

            var motorcycle = await _context.Motorcycles.FirstOrDefaultAsync(m => m.Plate == dto.Plate);
            
            if (motorcycle == null)
                throw new Exception("Moto não encontrada.");

            bool isMotorcycleAlredyRented = await _context.Rentals
                .AnyAsync(r => r.MotorcycleId == motorcycle.Id && r.EndDate == null);
            if (isMotorcycleAlredyRented)
                throw new Exception("Moto já está alocada");

            int days = (int)dto.Plan;
            decimal costPerDay = dto.Plan switch
            {
                RentalPlan.SevenDays => 30,
                RentalPlan.FifteenDays => 28,
                RentalPlan.ThirtyDays => 22,
                RentalPlan.FortyFiveDays => 20,
                RentalPlan.FiftyDays => 18,
                _ => throw new Exception("Plano inválido.")
            };

            var rental = new Rental
            {
                UserId = userId,
                MotorcycleId = motorcycle.Id,
                StartDate = DateTime.UtcNow.Date.AddDays(1),
                ExpectedEndDate = DateTime.UtcNow.Date.AddDays(1 + days),
                TotalCost = days * costPerDay
            };

            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();

            return rental;
        }

        public async Task<Rental> ReturnRentalAsync(Guid userId, ReturnRentalDto dto)
        {
            var motorcycle = await _context.Motorcycles.FirstOrDefaultAsync(m => m.Plate == dto.Plate);
            if (motorcycle == null)
                throw new Exception("Moto não encontrada.");

            var rental = await _context.Rentals
                .FirstOrDefaultAsync(r => r.MotorcycleId == motorcycle.Id && r.UserId == userId && r.EndDate == null);
            if (rental == null)
                throw new Exception("Locação ativa não encontrada para esta moto e usuário.");

            int totalDaysPlanned = (rental.ExpectedEndDate - rental.StartDate).Days;
            int totalDaysUsed = (dto.ReturnDate - rental.StartDate).Days;
            totalDaysUsed = totalDaysUsed <= 0 ? 1 : totalDaysUsed;

            decimal dailyRate = rental.TotalCost / totalDaysPlanned;
            decimal totalCost = 0;

            if (dto.ReturnDate < rental.ExpectedEndDate)
            {
                int unusedDays = totalDaysPlanned - totalDaysUsed;
                decimal penaltyPercentage = totalDaysPlanned switch
                {
                    7 => 0.20m,
                    15 => 0.40m,
                    30 => 0.50m,
                    45 => 0.50m,
                    50 => 0.50m,
                    _ => 0m
                };
                totalCost = (dailyRate * totalDaysUsed) + (dailyRate * unusedDays * penaltyPercentage);
            }

            else if (dto.ReturnDate > rental.ExpectedEndDate)
            {
                int extraDays = (dto.ReturnDate - rental.ExpectedEndDate).Days;
                totalCost = rental.TotalCost + (extraDays * 50);
            }
            else
            {
                totalCost = rental.TotalCost;
            }

            rental.EndDate = dto.ReturnDate;
            rental.TotalCost = totalCost;

            await _context.SaveChangesAsync();

            return rental;
        }
    }
}
