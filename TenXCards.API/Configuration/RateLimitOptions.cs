namespace TenXCards.API.Options
{
    public class RateLimitOptions
    {
        public const string SectionName = "RateLimit";

        public int WindowInMinutes { get; set; } = 1;
        public int MaxRequestsPerWindow { get; set; } = 30;
    }
}