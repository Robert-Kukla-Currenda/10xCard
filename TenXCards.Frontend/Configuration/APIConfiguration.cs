namespace TenXCards.Frontend.Configuration
{
    public class APIConfiguration
    {
        public const string SectionName = "API";

        public required string Url { get; set; } = default!;
        public required JwtOptions Jwt { get; set; } = default!;

        public class JwtOptions
        {
            public required string Issuer { get; set; } = default!;
            public required string Audience { get; set; } = default!;
        }
    }
}
