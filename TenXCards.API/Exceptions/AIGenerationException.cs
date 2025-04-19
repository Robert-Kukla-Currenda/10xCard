namespace TenXCards.API.Exceptions;

public class AIGenerationException : Exception
{
    public AIGenerationException(string message) : base(message)
    {
    }

    public AIGenerationException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}