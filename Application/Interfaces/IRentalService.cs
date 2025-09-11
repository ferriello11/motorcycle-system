using Microsoft.AspNetCore.Mvc;
using TesteTecnico.Application.DTOs;
using TesteTecnico.Domain.Entities;

namespace TesteTecnico.Application.Interfaces
{
    public interface IRentalService
    {
        Task<Rental> CreateRentalAsync(Guid userId, CreateRentalDto dto);
        Task<Rental> ReturnRentalAsync(Guid userId, ReturnRentalDto dto);
    }
}
