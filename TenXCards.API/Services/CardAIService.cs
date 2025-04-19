using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.Extensions.Options;
using TenXCards.API.Configuration;
using TenXCards.API.Data;
using TenXCards.API.Exceptions;
using TenXCards.API.Models;

namespace TenXCards.API.Services;

public class CardAIService : ICardAIService
{
    private readonly HttpClient _httpClient;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<CardAIService> _logger;
    private readonly AIServiceOptions _options;

    public CardAIService(
        HttpClient httpClient,
        ApplicationDbContext dbContext,
        ILogger<CardAIService> logger,
        IOptions<AIServiceOptions> options)
    {
        _httpClient = httpClient;
        _dbContext = dbContext;
        _logger = logger;
        _options = options.Value;
        
        // Konfiguracja HttpClient dla OpenRouter
        _httpClient.BaseAddress = new Uri(_options.OpenRouterUrl);
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.OpenRouterApiKey}");
    }

    public async Task<CardDto> GenerateCardAsync(GenerateCardCommand command, int userId)
    {
        // Walidacja wejścia
        if (string.IsNullOrEmpty(command.OriginalContent))
        {
            throw new ValidationException("Original content cannot be empty");
        }

        if (command.OriginalContent.Length < 1000 || command.OriginalContent.Length > 10000)
        {
            throw new ValidationException("Original content must be between 1000 and 10000 characters");
        }

        try
        {
            // Generowanie fiszki przy użyciu AI
            var (front, back) = await GenerateCardContentAsync(command.OriginalContent);
            //todo usunać zapis do bazy
            // Tworzenie nowej fiszki
            var card = new Card
            {
                UserId = userId,
                OriginalContent = command.OriginalContent,
                Front = front,
                Back = back,
                GeneratedBy = "AI",
                CreatedAt = DateTime.UtcNow
            };

            // Zapis do bazy danych
            _dbContext.Cards.Add(card);
            await _dbContext.SaveChangesAsync();

            // Mapowanie na DTO
            return new CardDto
            {
                Id = card.Id,
                UserId = card.UserId,
                OriginalContent = card.OriginalContent,
                Front = card.Front,
                Back = card.Back,
                GeneratedBy = card.GeneratedBy,
                CreatedAt = card.CreatedAt
            };
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogError(ex, "AI service communication error");
            throw new AIGenerationException("Failed to communicate with AI service", ex);
        }
    }

    private async Task<(string front, string back)> GenerateCardContentAsync(string originalContent)
    {
        var prompt = $@"
Przeanalizuj poniższy tekst i stwórz fiszkę do nauki:
1. Na przedniej stronie umieść krótkie, zwięzłe pytanie (maksymalnie 1000 znaków).
2. Na tylnej stronie umieść szczegółową odpowiedź (maksymalnie 5000 znaków).

Tekst do analizy:
{originalContent}

Odpowiedź zwróć w formacie JSON:
{{
    ""front"": ""pytanie"",
    ""back"": ""odpowiedź""
}}";

        var request = new
        {
            model = _options.ModelName,
            messages = new[]
            {
                new { role = "system", content = "Jesteś ekspertem w tworzeniu fiszek edukacyjnych." },
                new { role = "user", content = prompt }
            }
        };

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(_options.TimeoutSeconds));
        
        var response = await _httpClient.PostAsJsonAsync("v1/chat/completions", request, cts.Token);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AIResponse>();
        
        if (result?.Choices == null || result.Choices.Length == 0)
        {
            throw new AIGenerationException("AI service returned no results");
        }

        try
        {
            // Parsowanie odpowiedzi JSON z AI
            var cardContent = JsonSerializer.Deserialize<CardContent>(
                result.Choices[0].Message.Content,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (cardContent == null || 
                string.IsNullOrEmpty(cardContent.Front) || 
                string.IsNullOrEmpty(cardContent.Back))
            {
                throw new AIGenerationException("AI generated invalid card content");
            }

            return (cardContent.Front, cardContent.Back);
        }
        catch (JsonException ex)
        {
            throw new AIGenerationException("Failed to parse AI response", ex);
        }
    }

    private record CardContent(string Front, string Back);

    private class AIResponse
    {
        public Choice[] Choices { get; set; } = Array.Empty<Choice>();
        
        public class Choice
        {
            public Message Message { get; set; } = new();
        }
        
        public class Message
        {
            public string Content { get; set; } = string.Empty;
        }
    }
}