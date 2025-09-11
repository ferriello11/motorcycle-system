using TesteTecnico.Application.Interfaces;
using TesteTecnico.Domain.Entities;
using TesteTecnico.Infrastructure.Data;

namespace TesteTecnico.Application.Services
{
    public class MotorcycleNotificationService : IMotorcycleNotificationService
    {
        private readonly AppDbContext _context;

        public MotorcycleNotificationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(MotorcycleNotification notification)
        {
            _context.MotorcycleNotifications.Add(notification);
            await _context.SaveChangesAsync();
        }
    }
}