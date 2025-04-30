using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenXCards.API.Exceptions;
using TenXCards.API.Models;
using TenXCards.API.Services;

namespace TenXCards.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CardsController : ControllerBase
{
    private readonly ICardService _cardService;
    private readonly ILogger<CardsController> _logger;

    public CardsController(ICardService cardService, ILogger<CardsController> logger)
    {
        _cardService = cardService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CardDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> CreateCard([FromBody] SaveCardCommand command)
    {
        // Get user ID from claims
        var userIdAsString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdAsString) || !int.TryParse(userIdAsString, out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var card = await _cardService.CreateCardAsync(command, userId);
            return CreatedAtAction(
                nameof(GetCard), 
                new { id = card.Id }, 
                card);
        }
        catch (ValidationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Generates a new flashcard using AI based on provided text
    /// </summary>
    /// <param name="command">The command containing text to process</param>
    /// <returns>The generated flashcard</returns>
    /// <response code="201">Card was successfully created</response>
    /// <response code="400">Invalid input data</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="422">AI service failed to generate card</response>
    [HttpPost("generate")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<CardDto>> GenerateCard([FromBody] GenerateCardCommand command)
    {
        try
        {
            // Get user ID from claims
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                ?? throw new UnauthorizedAccessException("User ID not found in claims"));

            // Generate card using AI service
            var card = await _cardService.GenerateCardAsync(command, userId);

            // Return created card
            return CreatedAtAction(nameof(GenerateCard), new { id = card.Id }, card);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation failed for card generation");
            return BadRequest(ex.Message);
        }
        catch (AIGenerationException ex)
        {
            _logger.LogError(ex, "AI service failed to generate card");
            return UnprocessableEntity("Failed to generate card using AI service");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during card generation");
            return StatusCode(500, "An unexpected error occurred while generating the card");
        }
    }

    // Placeholder for GetCard action (needed for CreatedAtAction)
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCard(int id)
    {
        // Implementation will be added later
        throw new NotImplementedException();
    }
}