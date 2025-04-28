using TenXCards.API.Data;
using TenXCards.API.Data.Models;
using TenXCards.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using TenXCards.API.Exceptions;
using TenXCards.API.Jwt;
using BC=BCrypt.Net.BCrypt;

namespace TenXCards.API.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<UserService> _logger;
        private readonly IJwtService _jwtService;

        public UserService(ApplicationDbContext dbContext, ILogger<UserService> logger, IJwtService jwtService)
        {
            _dbContext = dbContext;
            _logger = logger;
            _jwtService = jwtService;
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

        public async Task<LoginResultDto> Login(LoginUserCommand command)
        {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == command.Email);

            if (user == null || !BC.Verify(command.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var token = _jwtService.GenerateToken(user);
            
            return new LoginResultDto
            {
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    CreatedAt = user.CreatedAt
                }
            };
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            try
            {
                var user = await _dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id);

                return user != null ? MapToDto(user) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID {UserId}", id);
                throw;
            }
        }

        // Metoda pomocnicza do hashowania hasła
        private string HashPassword(string password)
        {
            return BC.HashPassword(password);
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