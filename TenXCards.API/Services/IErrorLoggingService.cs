namespace TenXCards.API.Services
{
    public interface IErrorLoggingService
    {
        Task LogErrorAsync(int cardId, Exception ex, CancellationToken cancellationToken = default);
    }
}