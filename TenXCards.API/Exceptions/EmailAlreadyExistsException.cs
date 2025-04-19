using System;

namespace TenXCards.API.Exceptions
{
    /// <summary>
    /// Wyjątek zgłaszany, gdy próbuje się zarejestrować użytkownika z istniejącym emailem
    /// </summary>
    public class EmailAlreadyExistsException : Exception
    {
        public EmailAlreadyExistsException(string message) : base(message)
        {
        }

        public EmailAlreadyExistsException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}