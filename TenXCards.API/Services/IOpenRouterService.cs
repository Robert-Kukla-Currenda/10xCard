using TenXCards.API.Models.OpenRouter;

namespace TenXCards.API.Services
{
    public interface IOpenRouterService
    {
        /// <summary>
        /// Sends a message to the OpenRouter API and returns the response.
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="role">The role of the message sender.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>The response from the OpenRouter API.</returns>
        /// <exception cref="ValidationException">Thrown when the message is empty or null.</exception>
        Task<string> SendMessageAsync(Prompt prompt);
    }
}