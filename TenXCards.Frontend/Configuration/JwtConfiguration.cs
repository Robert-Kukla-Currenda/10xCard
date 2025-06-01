namespace TenXCards.Frontend.Configuration
{
    public class JwtConfiguration
    {
        public const string SectionName = "JWT";

        public required string Issuer { get; set; } = default!;
        public required string Audience { get; set; } = default!;
    }
}
