using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TenXCards.API.Configuration;
using TenXCards.API.Controllers;
using TenXCards.API.Data;
using TenXCards.API.Data.Models;
using TenXCards.API.Exceptions;
using TenXCards.API.Models;
using TenXCards.API.Models.OpenRouter;

namespace TenXCards.API.Services;

public class CardService : ICardService
{
    private const string CACHE_CARD_PREFIX = "card_{userId}";

    private readonly HttpClient _httpClient;
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<CardService> _logger;
    private readonly AIServiceOptions _options;
    private readonly IMemoryCache _cache;
    private readonly IOptions<CacheOptions> _cacheOptions;
    private readonly IOpenRouterService _openRouterService;

    public CardService(
        HttpClient httpClient,
        ApplicationDbContext dbContext,
        ILogger<CardService> logger,
        IOptions<AIServiceOptions> options,
        IMemoryCache cache,
        IOptions<CacheOptions> cacheOptions,
        IOpenRouterService openRouterService)
    {
        _httpClient = httpClient;
        _dbContext = dbContext;
        _logger = logger;
        _options = options.Value;
        _cache = cache;
        _cacheOptions = cacheOptions;
        _openRouterService = openRouterService;

        // Konfiguracja HttpClient dla OpenRouter
        //_httpClient.BaseAddress = new Uri(_options.OpenRouterUrl);
        //_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_options.OpenRouterApiKey}");
    }

    #region Save Card
    public async Task<CardDto> CreateCardAsync(SaveCardCommand command, int userId)
    {
        var transaction = _dbContext.Database.BeginTransaction();
        try
        {
            var card = new Card
            {
                UserId = userId,
                Front = command.Front,
                Back = command.Back,
                GeneratedBy = command.GeneratedBy,
                OriginalContentId = command.OriginalContentId,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Cards.Add(card);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            // Invalidate cache for this user's card lists
            InvalidateUserCardListCache(userId);

            return await GetCardByIdAsync(card.Id, userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating card for user {UserId}", userId);
            throw new ApplicationException("Failed to create card", ex);
        }*/
        return null;
    }
    }
    #endregion

    #region Get Cards
    public async Task<PaginatedResult<CardDto>> GetCardsAsync(GetPagedListQuery query, int userId)
    {
        try
        {
            // Validate page and limit
            if (query.Page < 1)
                throw new ValidationException("Page must be greater than 0");

            if (query.Limit < 1 || query.Limit > 100)
                throw new ValidationException("Limit must be between 1 and 100");

            // Generate cache key based on query parameters
            var cacheKey = GenerateCacheKeyForGetList(query, userId);

            // Try to get from cache first
            if (_cache.TryGetValue<PaginatedResult<CardDto>>(cacheKey, out var cachedResult) && cachedResult != null)
            {
                _logger.LogDebug("Cache hit for key: {CacheKey}", cacheKey);
                return cachedResult;
            }

            // If not in cache, get from database
            var result = await GetCardsFromDatabase(query, userId);

            // Cache the result if it's not too large
            if (result.Total <= _cacheOptions.Value.MaxCardListItems)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(_cacheOptions.Value.CardListExpirationMinutes))
                    .SetSize(1); // Each entry counts as 1 unit

                _cache.Set(cacheKey, result, cacheEntryOptions);
                _logger.LogDebug("Cached results for key: {CacheKey}", cacheKey);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cards for user {UserId}", userId);
            throw;
        }
    }
    #endregion

    #region Get Single Card
    public async Task<CardDto> GetCardByIdAsync(int cardId, int userId)
    {
        /*
        try
        {
            // Try to get from cache first
            var cacheKey = $"card_{userId}_{cardId}";

            if (_cache.TryGetValue<CardDto>(cacheKey, out var cachedCard) && cachedCard != null)
            {
                _logger.LogDebug("Cache hit for card {CardId}", cardId);
                return cachedCard;
            }

            var card = await _dbContext.Cards
                .Where(c => c.Id == cardId && c.UserId == userId)
                .Select(c => new CardDto
                {
                    Id = c.Id,
                    User = new UserDto
                    {
                        Id = c.UserId,
                        FirstName = c.User.FirstName,
                        LastName = c.User.LastName,
                        CreatedAt = c.User.CreatedAt,
                        Email = c.User.Email
                    },
                    Front = c.Front,
                    Back = c.Back,
                    GeneratedBy = c.GeneratedBy,
                    OriginalContent = new OriginalContentDto
                    {
                        Id = c.OriginalContentId,
                        Content = c.OriginalContent.Content
                    },
                    CreatedAt = c.CreatedAt                    
                })
                .FirstOrDefaultAsync();

            if (card == null)
            {
                throw new KeyNotFoundException($"Card with ID {cardId} not found");
            }

            return card;
        }
        catch (Exception ex) when (ex is not KeyNotFoundException)
        {
            _logger.LogError(ex, "Error getting card {CardId} for user {UserId}", cardId, userId);
            throw new ApplicationException("Failed to retrieve card", ex);
        }*/
        return null;
    }
    }
    #endregion

    #region Update Card
    public async Task<CardDto> UpdateCardAsync(int cardId, UpdateCardCommand command, int userId)
    {
        /*
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var card = await _dbContext.Cards
                .Where(c => c.Id == cardId && c.UserId == userId)
                .FirstOrDefaultAsync();

            if (card == null)
            {
                throw new KeyNotFoundException($"Card with ID {cardId} not found");
            }

            card.Front = command.Front;
            card.Back = command.Back;
            //card.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            // Invalidate cache for this card and user's card lists
            InvalidateUserCardListCache(userId);
            InvalidateUserSingleCardCache(userId, cardId);

            return await GetCardByIdAsync(card.Id, userId);
        }
        catch (Exception ex) when (ex is not KeyNotFoundException)
        {
            _logger.LogError(ex, "Error updating card {CardId} for user {UserId}", cardId, userId);
            throw new ApplicationException("Failed to update card", ex);
        }
        */
        return null;
    }
    #endregion
    
    #region Delete Card
    public async Task DeleteCardAsync(int cardId, int userId)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var card = await _dbContext.Cards
                .Where(c => c.Id == cardId && c.UserId == userId)
                .FirstOrDefaultAsync();

            if (card == null)
            {
                throw new KeyNotFoundException($"Card with ID {cardId} not found");
            }

            _dbContext.Cards.Remove(card);
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            // Invalidate cache for this card and user's card lists
            InvalidateUserCardListCache(userId);            
            InvalidateUserSingleCardCache(userId, cardId);

            _logger.LogInformation("Card {CardId} deleted successfully by user {UserId}", cardId, userId);
        }
        catch (Exception ex) when (ex is not KeyNotFoundException)
        {
            _logger.LogError(ex, "Error deleting card {CardId} for user {UserId}", cardId, userId);
            throw new ApplicationException("Failed to delete card", ex);
        }
    }
    #endregion

    #region AI Card Generation
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
            var x = await _openRouterService.SendMessageAsync(command.OriginalContent, MessageRole.User);

            // Generowanie fiszki przy użyciu AI
            //var (front, back) = await GenerateCardContentAsync(command.OriginalContent);

            return new CardDto() { Back = x };
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
            model = _options.OpenRouterModelName,
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
    #endregion

    #region Caching
    private string GenerateCacheKeyForGetList(GetPagedListQuery query, int userId) =>
        $"{CACHE_CARD_PREFIX}_list_{query.Page}_{query.Limit}_{query.GeneratedBy}_{query.Sort}"
        .Replace("{userId}", $"{userId}");

    private async Task<PaginatedResult<CardDto>> GetCardsFromDatabase(GetPagedListQuery query, int userId)
    {
        /*
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
            _ => cardsQuery.OrderByDescending(c => c.CreatedAt)
        };

        var total = await cardsQuery.CountAsync();

        var cards = await cardsQuery
            .Skip((query.Page - 1) * query.Limit)
            .Take(query.Limit)
            .Select(c => new CardDto
            {
                Id = c.Id,
                User = new UserDto
                {
                    Id = c.UserId,
                    FirstName = c.User.FirstName,
                    LastName = c.User.LastName,
                    CreatedAt = c.User.CreatedAt,
                    Email = c.User.Email
                },
                Front = c.Front,
                Back = c.Back,
                GeneratedBy = c.GeneratedBy,
                OriginalContent = new OriginalContentDto
                {
                    Id = c.OriginalContentId,
                    Content = c.OriginalContent.Content
                },
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
        */
        return null;
    }

    private void InvalidateUserCardListCache(int userId)
    {
        var memoryCacheKeys = ((MemoryCache)_cache).Keys;
        var pattern = $"{CACHE_CARD_PREFIX}".Replace("{userId}", $"{userId}");

        var removedKeysCount = 0;
        foreach (var key in memoryCacheKeys)
        {
            if (key != null && key.ToString()!.StartsWith(pattern))
            {
                _cache.Remove(key);
                removedKeysCount++;
            }
        }

        _logger.LogDebug("Invalidated {Count} cache entries for user {UserId}", removedKeysCount, userId);
    }

    private void InvalidateUserSingleCardCache(int userId, int cardId)
    {
        var cacheKey = $"{CACHE_CARD_PREFIX}_{cardId}"
            .Replace("{userId}", $"{userId}"); ;
        _cache.Remove(cacheKey);
        _logger.LogDebug("Invalidated cache entry for card {CardId} of user {UserId}", cardId, userId);
    }
    #endregion
}