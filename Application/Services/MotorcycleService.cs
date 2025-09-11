using TesteTecnico.Application.DTOs;
using TesteTecnico.Application.Interfaces;
using TesteTecnico.Domain.Entities;
using TesteTecnico.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;
using TesteTecnico.Application.Validators;

namespace TesteTecnico.Application.Services
{
    public class MotorcycleService : IMotorcycleService
    {
        private readonly AppDbContext _context;
        private readonly IAmazonSQS _sqsClient;
        private readonly string _queueUrl;

        public MotorcycleService(AppDbContext context, IAmazonSQS sqsClient, IConfiguration config)
        {
            _context = context;
            _sqsClient = sqsClient;
            _queueUrl = config["AWS:SQSQueueUrl"] ?? throw new Exception("SQS Queue URL não configurada");
        }

        public async Task<Motorcycle> CreateAsync(CreateMotorcycleDto dto)
        {
            await ServiceValidator.ValidateUniqueMotorcyclePlateAsync(_context, dto.Plate);

            var moto = new Motorcycle
            {
                Identifier = dto.Identifier,
                Model = dto.Model,
                Plate = dto.Plate,
                Year = dto.Year
            };

            _context.Motorcycles.Add(moto);
            await _context.SaveChangesAsync();

            var messageBody = JsonSerializer.Serialize(new
            {
                moto.Identifier,
                moto.Plate,
                moto.Model,
                moto.Year
            });

            var message = new SendMessageRequest
            {
                QueueUrl = _queueUrl,
                MessageBody = messageBody
            };

            await _sqsClient.SendMessageAsync(message);

            return moto;
        }

        public async Task<IEnumerable<Motorcycle>> GetAllAsync(string? plate = null)
        {
            var query = _context.Motorcycles.AsQueryable();
            if (!string.IsNullOrEmpty(plate))
                query = query.Where(m => m.Plate == plate);

            return await query.ToListAsync();
        }

        public async Task<Motorcycle> UpdatePlateAsync(string oldPlate, string newPlate)
        {
            var moto = await _context.Motorcycles.FirstOrDefaultAsync(m => m.Plate == oldPlate);
            await ServiceValidator.ValidateMotorcycleExists(_context, oldPlate);
            await ServiceValidator.ValidateUniqueMotorcyclePlateAsync(_context, newPlate);

            moto.Plate = newPlate;
            await _context.SaveChangesAsync();

            return moto;
        }

        public async Task DeleteAsync(string plate)
        {
            var moto = await _context.Motorcycles
                .FirstOrDefaultAsync(m => m.Plate == plate);

            await ServiceValidator.ValidateMotorcycleExists(_context, moto.Plate);

            bool hasRentals = await _context.Rentals
                .AnyAsync(r => r.MotorcycleId == moto.Id);

            if (hasRentals)
                throw new Exception("Não é possível deletar a moto, existem locações associadas.");


            _context.Motorcycles.Remove(moto);
            await _context.SaveChangesAsync();
        }
    }
}
