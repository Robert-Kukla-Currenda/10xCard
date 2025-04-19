using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using TenXCards.API.Exceptions;
using TenXCards.API.Models;
using TenXCards.API.Services;

namespace TenXCards.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Rejestracja nowego użytkownika
        /// </summary>
        /// <param name="command">Dane użytkownika do rejestracji</param>
        /// <returns>Dane nowo utworzonego użytkownika</returns>
        /// <response code="201">Użytkownik został pomyślnie utworzony</response>
        /// <response code="400">Nieprawidłowe dane wejściowe</response>
        /// <response code="409">Użytkownik o podanym adresie email już istnieje</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [Description("Rejestracja nowego użytkownika")]
        public async Task<ActionResult<UserDto>> Register(RegisterUserCommand command)
        {
            // Guard clauses - walidacja modelu jest wykonywana automatycznie przez [ApiController]
            
            try
            {
                // Rejestracja użytkownika
                var user = await _userService.RegisterUserAsync(command);
                
                // Zwróć wynik z kodem 201 Created
                return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
            }
            catch (EmailAlreadyExistsException ex)
            {
                // Email już istnieje - konflikt
                _logger.LogWarning(ex, "Próba rejestracji z istniejącym adresem email: {Email}", command.Email);
                return Conflict(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                // Nieoczekiwany błąd
                _logger.LogError(ex, "Błąd podczas rejestracji użytkownika: {Email}", command.Email);
                return StatusCode(500, new { error = "Wystąpił nieoczekiwany błąd podczas rejestracji." });
            }
        }
    }
}