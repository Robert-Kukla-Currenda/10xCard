namespace TenXCards.API.Configuration;

public class AIServiceOptions
{
    public const string ConfigurationSection = "AIService";
    
    public required string OpenRouterApiKey { get; set; }
    public required string OpenRouterUrl { get; set; }
    public required string ModelName { get; set; }
    public int MaxRetries { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 30;
}