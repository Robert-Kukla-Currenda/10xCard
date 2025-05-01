namespace TenXCards.API.Configuration
{
    public class CacheOptions
    {
        public const string SectionName = "Cache";
        public int CardListExpirationMinutes { get; set; } = 5;
        public int MaxCardListItems { get; set; } = 1000;
    }
}