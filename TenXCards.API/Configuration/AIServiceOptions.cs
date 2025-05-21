namespace TenXCards.API.Configuration;

public class AIServiceOptions
{
    public const string SectionName = "AIService";
    
    public required string OpenRouterApiKey { get; set; }
    public required string OpenRouterUrl { get; set; } = "https://openrouter.ai/api/v1/chat/completions";
    public required string OpenRouterModelName { get; set; } = "openai/gpt-4o-mini";
    public required double ModelTemperature { get; set; } = 0.7;
    public required int ModelMaxTokens { get; set; } = 1000;
    public int MaxRetries { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 30;
}