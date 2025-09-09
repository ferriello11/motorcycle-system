using Microsoft.EntityFrameworkCore;
using TesteTecnico.Domain.Entities;

namespace TesteTecnico.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        
        public DbSet<User> Users { get; set; }
         
        public DbSet<Motorcycle> Motorcycles { get; set; }

        public DbSet<MotorcycleNotification> MotorcycleNotifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Motorcycle>()
                .HasIndex(m => m.Plate)
                .IsUnique();            
        }
        
    }
}