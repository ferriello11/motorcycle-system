using TesteTecnico.Domain.Entities;

namespace TesteTecnico.Application.Interfaces
{
    public interface IMotorcycleNotificationService
    {
        Task SaveAsync(MotorcycleNotification notification);
    }
}