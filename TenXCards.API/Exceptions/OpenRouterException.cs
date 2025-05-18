namespace TenXCards.API.Exceptions
{
    public class OpenRouterException : Exception
    {
        public string Code { get; }
        public object? Details { get; }

        public OpenRouterException(string message, string code, object? details = null) 
            : base(message)
        {
            Code = code;
            Details = details;
        }
    }

    public class NetworkException : OpenRouterException
    {
        public NetworkException(string message, object? details = null) 
            : base(message, "NETWORK_ERROR", details)
        { }
    }

    public class AuthorizationException : OpenRouterException
    {
        public AuthorizationException(string message, object? details = null) 
            : base(message, "AUTH_ERROR", details)
        { }
    }
}