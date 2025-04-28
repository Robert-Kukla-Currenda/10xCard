using TenXCards.API.Models;

namespace TenXCards.API.Services
{
    public interface IUserService
    {
        /// <summary>
        /// Rejestruje nowego użytkownika w systemie
        /// </summary>
        /// <param name="command">Dane nowego użytkownika</param>
        /// <returns>Dane utworzonego użytkownika</returns>
        /// <exception cref="EmailAlreadyExistsException">Gdy użytkownik o podanym emailu już istnieje</exception>
        Task<UserDto> RegisterUserAsync(RegisterUserCommand command);

        Task<LoginResultDto> Login(LoginUserCommand command);

        /// <summary>
        /// Retrieves a user by their ID
        /// </summary>
        /// <param name="id">The user's ID</param>
        /// <returns>UserDto if found, null otherwise</returns>
        Task<UserDto?> GetUserByIdAsync(int id);
    }
}