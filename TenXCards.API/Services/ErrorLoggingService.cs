using Microsoft.EntityFrameworkCore;
using TenXCards.API.Data;
using TenXCards.API.Data.Models;

namespace TenXCards.API.Services
{
    public class ErrorLoggingService : IErrorLoggingService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ErrorLoggingService> _logger;

        public ErrorLoggingService(
            ApplicationDbContext dbContext,
            ILogger<ErrorLoggingService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task LogErrorAsync(
            int cardId, 
            Exception ex, 
            CancellationToken cancellationToken = default)
        {
            var errorLog = new ErrorLog
            {
                CardId = cardId,
                ErrorDetails = ex.Message,
                LoggedAt = DateTime.UtcNow                
            };

            try
            {
                _dbContext.ErrorLogs.Add(errorLog);
                await _dbContext.SaveChangesAsync(cancellationToken);
                
                _logger.LogError(
                    ex,
                    "Card generation error - CardId: {CardId}, Error: {ErrorMessage}",
                    cardId,
                    ex.Message);
            }
            catch (Exception dbEx)
            {
                _logger.LogCritical(
                    dbEx,
                    "Failed to log error to database - CardId: {CardId}, Original error: {ErrorMessage}",
                    cardId,
                    ex.Message);
            }
        }
    }
}