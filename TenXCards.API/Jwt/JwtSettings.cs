namespace TenXCards.API.Jwt
{
    public class JwtSettings
    {
        public string SecretKey { get; set; } = default!;
        public int ExpiryMinutes { get; set; }
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
    }
}