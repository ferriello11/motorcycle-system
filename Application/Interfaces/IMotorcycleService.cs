using TesteTecnico.Domain.Entities;
using TesteTecnico.Application.DTOs;

namespace TesteTecnico.Application.Interfaces
{
    public interface IMotorcycleService
    {
        Task<Motorcycle> CreateAsync(CreateMotorcycleDto dto);
        Task<IEnumerable<Motorcycle>> GetAllAsync(string? plate = null);
        Task<Motorcycle> UpdatePlateAsync(string oldPlate, string newPlate);
        Task DeleteAsync(string plate);
    }
}
