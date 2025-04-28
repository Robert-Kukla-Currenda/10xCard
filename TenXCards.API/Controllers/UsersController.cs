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


        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResultDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResultDto>> Login([FromBody] LoginUserCommand command)
        {
            try
            {
                var result = await _userService.Login(command);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Failed login attempt for user {Email}", command.Email);
                return Unauthorized(new { message = "Invalid credentials" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Email}", command.Email);
                return StatusCode(500, new { message = "An error occurred during login" });
            }
        }
    }
}