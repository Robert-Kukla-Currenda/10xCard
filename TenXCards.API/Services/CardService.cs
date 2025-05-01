using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TenXCards.API.Configuration;
using TenXCards.API.Controllers;
using TenXCards.API.Data;
using TenXCards.API.Data.Models;
using TenXCards.API.Exceptions;
using TenXCards.API.Models;

namespace TenXCards.API.Services;

public class CardService : ICardService
{
    private readonly HttpClient _httpClient;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<CardService> _logger;
    private readonly AIServiceOptions _options;

    public CardService(
        HttpClient httpClient,
        ApplicationDbContext dbContext,
        ILogger<CardService> logger,
        IOptions<AIServiceOptions> options)
    {
        _httpClient = httpClient;
        _dbContext = dbContext;
        _logger = logger;
        _options = options.Value;

        // Konfiguracja HttpClient dla OpenRouter
        //_httpClient.BaseAddress = new Uri(_options.OpenRouterUrl);
        //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.OpenRouterApiKey}");
    }

    public async Task<CardDto> CreateCardAsync(SaveCardCommand command, int userId)
    {
        try
        {
            var card = new Card
            {
                UserId = userId,
                Front = command.Front,
                Back = command.Back,
                GeneratedBy = command.GeneratedBy,
                OriginalContent = command.OriginalContent,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Cards.Add(card);
            await _dbContext.SaveChangesAsync();

            return new CardDto
            {
                Id = card.Id,
                UserId = card.UserId,
                Front = card.Front,
                Back = card.Back,
                GeneratedBy = card.GeneratedBy,
                OriginalContent = card.OriginalContent,
                CreatedAt = card.CreatedAt,
                UpdatedAt = null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating card for user {UserId}", userId);
            throw new ApplicationException("Failed to create card", ex);
        }
    }

    public async Task<PaginatedResult<CardDto>> GetCardsAsync(GetCardsQuery query, int userId)
    {
        try
        {
            // Validate page and limit
            if (query.Page < 1)
                throw new ValidationException("Page must be greater than 0");

            if (query.Limit < 1 || query.Limit > 100)
                throw new ValidationException("Limit must be between 1 and 100");

            // Build the query
            var cardsQuery = _dbContext.Cards
                .Where(c => c.UserId == userId);

            // Apply filters
            if (!string.IsNullOrWhiteSpace(query.GeneratedBy))
            {
                cardsQuery = cardsQuery.Where(c => c.GeneratedBy == query.GeneratedBy);
            }

            // Apply sorting
            cardsQuery = query.Sort?.ToLower() switch
            {
                "created_at_desc" => cardsQuery.OrderByDescending(c => c.CreatedAt),
                "created_at_asc" => cardsQuery.OrderBy(c => c.CreatedAt),
                _ => cardsQuery.OrderByDescending(c => c.CreatedAt) // default sorting
            };

            // Get total count
            var total = await cardsQuery.CountAsync();

            // Apply pagination
            var cards = await cardsQuery
                .Skip((query.Page - 1) * query.Limit)
                .Take(query.Limit)
                .Select(c => new CardDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    Front = c.Front,
                    Back = c.Back,
                    GeneratedBy = c.GeneratedBy,
                    OriginalContent = c.OriginalContent,
                    CreatedAt = c.CreatedAt,
                    //UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();

            return new PaginatedResult<CardDto>
            {
                Items = cards,
                Page = query.Page,
                Limit = query.Limit,
                Total = total
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cards for user {UserId}", userId);
            throw new ApplicationException("Failed to get cards", ex);
        }
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

            return new CardDto();
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