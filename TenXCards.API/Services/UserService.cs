using TenXCards.API.Data;
using TenXCards.API.Data.Models;
using TenXCards.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using TenXCards.API.Exceptions;

namespace TenXCards.API.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext dbContext, ILogger<UserService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<UserDto> RegisterUserAsync(RegisterUserCommand command)
        {
            // Sprawdź czy email już istnieje
            var emailExists = await _dbContext.Users
                .AnyAsync(u => u.Email == command.Email);

            if (emailExists)
            {
                throw new EmailAlreadyExistsException("Użytkownik o podanym adresie email już istnieje.");
            }

            // Hashowanie hasła
            string passwordHash = HashPassword(command.Password);

            // Utworzenie nowego użytkownika
            var user = new User
            {
                Email = command.Email,
                FirstName = command.FirstName,
                LastName = command.LastName,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            // Dodanie do bazy danych
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Zarejestrowano nowego użytkownika: {Email}", command.Email);

            // Mapowanie na DTO
            return MapToDto(user);
        }

        // Metoda pomocnicza do hashowania hasła
        private string HashPassword(string password)
        {
            // Generowanie losowej soli
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hashowanie hasła z solą
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            // Format: {algorytm}.{iteracje}.{salt}.{hash}
            return $"PBKDF2.100000.{Convert.ToBase64String(salt)}.{hashed}";
        }

        // Metoda mapująca encję na DTO
        private UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedAt = user.CreatedAt
            };
        }
    }
}