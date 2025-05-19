using TenXCards.API.Controllers;
using TenXCards.API.Models;

namespace TenXCards.API.Services;

/// <summary>
/// Service responsible for AI-based flashcard generation
/// </summary>
public interface ICardService
{
    Task<CardDto> CreateCardAsync(SaveCardCommand command, int userId);
    Task<PaginatedResult<CardDto>> GetCardsAsync(GetPagedListQuery query, int userId);
    Task<CardDto> GetCardByIdAsync(int cardId, int userId);
    Task<CardDto> UpdateCardAsync(int cardId, UpdateCardCommand command, int userId);
    Task DeleteCardAsync(int cardId, int userId);

    /// <summary>
    /// Generates a flashcard using AI based on provided text input
    /// </summary>
    /// <param name="command">Command containing the original content</param>
    /// <param name="userId">ID of the user creating the card</param>
    /// <returns>Generated card data</returns>
    /// <exception cref="ValidationException">Thrown when input validation fails</exception>
    /// <exception cref="AIGenerationException">Thrown when AI service fails to generate card</exception>
    Task<CardDto> GenerateCardAsync(GenerateCardCommand command, int userId);
}