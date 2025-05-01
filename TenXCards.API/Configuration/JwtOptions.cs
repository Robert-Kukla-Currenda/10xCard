namespace TenXCards.API.Configuration
{
    public class JwtOptions
    {
        public const string SectionName = "JWT";

        public string SecretKey { get; set; } = default!;
        public int ExpiryMinutes { get; set; }
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
    }
}