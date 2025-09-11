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
        
        public DbSet<Rental> Rentals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Cnpj)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.CnhNumber)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.CnhType)
                .HasConversion<string>()
                .HasMaxLength(3);

            modelBuilder.Entity<Motorcycle>()
                .HasIndex(m => m.Plate)
                .IsUnique();

            modelBuilder.Entity<MotorcycleNotification>()
                .HasKey(mn => mn.Id);

            modelBuilder.Entity<MotorcycleNotification>()
                .Property(mn => mn.Plate)
                .IsRequired();

            modelBuilder.Entity<MotorcycleNotification>()
                .Property(mn => mn.Message)
                .IsRequired();

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Motorcycle)
                .WithMany()
                .HasForeignKey(r => r.MotorcycleId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}