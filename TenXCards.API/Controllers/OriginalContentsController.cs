using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenXCards.API.Attributes;
using TenXCards.API.Models;
using TenXCards.API.Services;

namespace TenXCards.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [RateLimit]
    public class OriginalContentsController : ControllerBase
    {
        private readonly IOriginalContentService _originalContentService;
        private readonly ILogger<OriginalContentsController> _logger;

        public OriginalContentsController(
            IOriginalContentService originalContentService,
            ILogger<OriginalContentsController> logger)
        {
            _originalContentService = originalContentService;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(typeof(OriginalContentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateOriginalContentCommand command)
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

                // Validate model (additional to DataAnnotations)
                if (string.IsNullOrWhiteSpace(command.Content))
                {
                    return BadRequest("Content cannot be empty");
                }

                // Create content through service
                var result = await _originalContentService.CreateAsync(command, userId);

                // Return 201 Created with the result
                return CreatedAtAction(
                    nameof(GetOriginalContent),
                    new { id = result.Id },
                    result);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Validation error while creating original content");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating original content");
                return StatusCode(StatusCodes.Status500InternalServerError,
                        new { error = "An error occurred while processing your request." });
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OriginalContentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OriginalContentDto>> GetOriginalContent(int id)
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

                var content = await _originalContentService.GetContentByIdAsync(id, userId);
                return Ok(content);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving original content with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the content");
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<OriginalContentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<PaginatedResult<OriginalContentDto>>> GetList(
            [FromQuery] int page = 1,
            [FromQuery] int limit = 20,
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

                var query = new GetPagedListQuery
                {
                    Page = page,
                    Limit = limit,
                    Sort = sort
                };

                var (items, total) = await _originalContentService.GetUserContentsAsync(query, userId);

                return Ok(new PaginatedResult<OriginalContentDto>
                {
                    Items = items,
                    Page = page,
                    Limit = limit,
                    Total = total
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving original content list");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int id)
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
                
                await _originalContentService.DeleteContentAsync(id, userId);

                return Ok(new { message = "Original content and associated cards deleted successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting original content {ContentId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the content." });
            }
        }

    }
}