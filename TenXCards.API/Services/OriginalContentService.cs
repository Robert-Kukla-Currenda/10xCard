using Microsoft.EntityFrameworkCore;
using TenXCards.API.Controllers;
using TenXCards.API.Data;
using TenXCards.API.Data.Models;
using TenXCards.API.Models;

namespace TenXCards.API.Services
{
    public class OriginalContentService : IOriginalContentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OriginalContentService> _logger;

        public OriginalContentService(ApplicationDbContext context, ILogger<OriginalContentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<OriginalContentDto> CreateAsync(CreateOriginalContentCommand command, int userId)
        {
            if (string.IsNullOrEmpty(command.Content))
                throw new ArgumentException("Content cannot be empty", nameof(command.Content));

            if (command.Content.Length < 1000 || command.Content.Length > 10000)
                throw new ArgumentException("Content length must be between 1000 and 10000 characters", nameof(command.Content));

            var entity = new OriginalContent
            {
                UserId = userId,
                Content = command.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.OriginalContents.Add(entity);
            await _context.SaveChangesAsync();

            return new OriginalContentDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Content = entity.Content,
                CreatedAt = entity.CreatedAt
            };
        }

        public async Task<OriginalContentDto> GetContentByIdAsync(int id, int userId)
        {
            var content = await _context.OriginalContents
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (content == null)
            {
                _logger.LogWarning("Original content with ID {Id} not found", id);
                throw new KeyNotFoundException($"Original content with ID {id} not found");
            }

            if (content.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to access content {ContentId} belonging to user {OwnerId}",
                    userId, id, content.UserId);
                throw new UnauthorizedAccessException("You do not have permission to access this content");
            }

            return new OriginalContentDto
            {
                Id = content.Id,
                UserId = content.UserId,
                Content = content.Content,
                CreatedAt = content.CreatedAt
            };
        }

        public async Task<(List<OriginalContentDto> Items, int Total)> GetUserContentsAsync(GetPagedListQuery getListQuery, int userId)
        {
            var query = _context.OriginalContents
                .Where(c => c.UserId == userId);

            // Apply sorting
            query = getListQuery.Sort switch
            {
                "created_at_desc" => query.OrderByDescending(c => c.CreatedAt),
                "created_at_asc" => query.OrderBy(c => c.CreatedAt),
                _ => query.OrderByDescending(c => c.CreatedAt) // default sorting
            };

            var total = await query.CountAsync();

            var items = await query
                .Skip((getListQuery.Page - 1) * getListQuery.Limit)
                .Take(getListQuery.Limit)
                .Select(c => new OriginalContentDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return (items, total);
        }

        public async Task DeleteContentAsync(int id, int userId)
        {
            var content = await _context.OriginalContents
                .Include(c => c.Cards)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (content == null)
            {
                _logger.LogWarning("Original content with ID {Id} not found", id);
                throw new KeyNotFoundException($"Original content with ID {id} not found");
            }

            if (content.UserId != userId)
            {
                _logger.LogWarning("User {UserId} attempted to delete content {ContentId} belonging to user {OwnerId}",
                    userId, id, content.UserId);
                throw new UnauthorizedAccessException("You do not have permission to delete this content");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Cards.RemoveRange(content.Cards);
                _context.OriginalContents.Remove(content);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("Successfully deleted original content {ContentId} and its {CardCount} associated cards",
                    id, content.Cards.Count);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting original content {ContentId}", id);
                throw;
            }
        }
    }
}