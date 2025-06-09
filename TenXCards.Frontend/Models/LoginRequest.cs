namespace TenXCards.Frontend.Models
{
    public class LoginRequest
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class LoginResponse
    {
        public required string Token { get; set; }        
        public DateTime ExpiresAt { get; set; }
    }
}
