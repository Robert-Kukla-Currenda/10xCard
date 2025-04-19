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
    }
}