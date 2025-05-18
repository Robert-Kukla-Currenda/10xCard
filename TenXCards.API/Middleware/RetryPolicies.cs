using Polly;
using Polly.Retry;

namespace FlashCards.Services.OpenRouter.Policies
{
    public static class RetryPolicies
    {
        public static AsyncRetryPolicy<HttpResponseMessage> GetHttpRetryPolicy(ILogger logger)
        {
            return Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .Or<TimeoutException>()
                .WaitAndRetryAsync(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                    logger.LogWarning("coś tutaj nie gra");
            //            logger.LogWarning(
            //                exception,
            //                "Retry {RetryCount} after {Delay}ms - {ErrorMessage}",
            //retryCount,
            //timeSpan.TotalMilliseconds, 
            //exception.Message);
            //todo Poprawić to exception.Message
        }
                );
        }
    }
}