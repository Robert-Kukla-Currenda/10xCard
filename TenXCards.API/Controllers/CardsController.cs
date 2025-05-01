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
    /// Gets a paginated list of cards for the authenticated user.
    /// </summary>
    /// <param name="page">Page number (starts from 1)</param>
    /// <param name="limit">Number of items per page (1-100)</param>
    /// <param name="generatedBy">Filter by generation type (AI or human)</param>
    /// <param name="sort">Sort order (created_at_desc, created_at_asc)</param>
    /// <returns>A paginated list of cards</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<CardDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCards(
        [FromQuery] int page = 1,
        [FromQuery] int limit = 20,
        [FromQuery] string? generatedBy = null,
        [FromQuery] string? sort = null)
    {
        try
        {
            // Get user ID from claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                _logger.LogWarning("User ID claim missing or invalid");
                return Unauthorized();
            }

            var query = new GetCardsQuery
            {
                Page = page,
                Limit = limit,
                GeneratedBy = generatedBy,
                Sort = sort
            };

            var result = await _cardService.GetCardsAsync(query, userId);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning(ex, "Validation error in GetCards");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting cards for user");
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "An error occurred while processing your request." });
        }
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CardDto>> GetCard([FromRoute] int id)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found in token"));

            var card = await _cardService.GetCardByIdAsync(id, userId);
            return Ok(card);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving card {CardId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the card" });
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
}