using TenXCards.API.Data.Models;

namespace TenXCards.API.Jwt
{
    /// <summary>
    /// Serwis do generowania tokenów JWT
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generuje token JWT dla użytkownika
        /// </summary>
        /// <param name="user">Użytkownik</param>
        /// <returns>Token JWT</returns>
        string GenerateToken(User user);
    }
}