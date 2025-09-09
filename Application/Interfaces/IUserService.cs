using TesteTecnico.Domain.Entities;

namespace TesteTecnico.Application.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterAsync(string name, string email, string password, string role);
        Task<string?> LoginAsync(string email, string password);
    }
}