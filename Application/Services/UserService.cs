using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TesteTecnico.Application.DTOs;
using TesteTecnico.Application.Interfaces;
using TesteTecnico.Application.Validators;
using TesteTecnico.Domain.Entities;
using TesteTecnico.Domain.Enums;
using TesteTecnico.Infrastructure.Data;

namespace TesteTecnico.Application.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IAmazonS3 _s3Client;

        public UserService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<User> RegisterAsync(RegisterDto dto)
        {
            await ServiceValidator.ValidateUniqueEmail(_context, dto.Email);
            await ServiceValidator.ValidateUniqueCnh(_context, dto.CnhNumber);
            await ServiceValidator.ValidateUniqueCnpj(_context, dto.Cnpj);

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role.ToString()
            };

            if (dto.Role == UserRole.Deliverer)
            {
                if (string.IsNullOrEmpty(dto.CnhNumber) || dto.CnhFile == null)
                    throw new Exception("CNH é obrigatória para entregador.");

                if (!Enum.IsDefined(typeof(CnhType), dto.CnhType))
                    throw new Exception("Tipo de CNH inválido.");

                if (dto.CnhFile != null)
                {
                    var extension = Path.GetExtension(dto.CnhFile.FileName)?.ToLowerInvariant();
                    if (extension is not ".png" and not ".bmp")
                        throw new Exception("Formato inválido. Apenas PNG ou BMP são aceitos.");
                }

                user.Cnpj = dto.Cnpj;
                user.DateOfBirth = dto.DateOfBirth;
                user.CnhNumber = dto.CnhNumber;
                user.CnhType = dto.CnhType;
                user.CnhImageUrl = await SaveCnhFile(dto.CnhFile);
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateUserAsync(Guid userId, UpdateUserDto dto)
        {
            var user = await _context.Users.FindAsync(userId);

            await ServiceValidator.ValidateUserExists(_context, userId);

            if (!string.IsNullOrEmpty(dto.Password))
                user.PasswordHash = dto.Password;

            if (user.Role == UserRole.Deliverer.ToString())
            {
                if (dto.CnhImageUpdate != null && dto.CnhImageUpdate.Length > 0)
                    user.CnhImageUrl = await SaveCnhFile(dto.CnhImageUpdate);
            }

            await _context.SaveChangesAsync();
        }

        private async Task<string> SaveCnhFile(IFormFile file)
        {
            var bucketName = _config["AWS:BucketName"];
            var region = _config["AWS:Region"];
            var accessKey = _config["AWS:AccessKey"];
            var secretKey = _config["AWS:SecretKey"];

            var bucketRegion = RegionEndpoint.GetBySystemName(region);
            using var s3Client = new AmazonS3Client(accessKey, secretKey, bucketRegion);
            var transferUtility = new TransferUtility(s3Client);

            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var key = $"cnh/{Guid.NewGuid()}";

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = memoryStream,
                Key = key,
                BucketName = bucketName,
                ContentType = file.ContentType,
                StorageClass = S3StorageClass.Standard
            };

            await transferUtility.UploadAsync(uploadRequest);

            return $"https://{bucketName}.s3.{region}.amazonaws.com/{key}";
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
