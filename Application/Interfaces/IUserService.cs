using TesteTecnico.Application.DTOs;
using TesteTecnico.Domain.Entities;

namespace TesteTecnico.Application.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterAsync(RegisterDto dto);
        Task UpdateUserAsync(Guid userId, UpdateUserDto dto);
        Task<string?> LoginAsync(string email, string password);
    }
}