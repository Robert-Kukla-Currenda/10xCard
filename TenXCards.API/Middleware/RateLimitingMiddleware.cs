using Microsoft.Extensions.Caching.Memory;

namespace TenXCards.API.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMemoryCache _cache;
        private readonly ILogger<RateLimitingMiddleware> _logger;

        public RateLimitingMiddleware(
            RequestDelegate next,
            IMemoryCache cache,
            ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _cache = cache;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;
            if (path?.Contains("/api/users/login") == true)
            {
                var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                var cacheKey = $"login-attempt:{ipAddress}";

                var attempts = _cache.GetOrCreate(cacheKey, entry =>
                {
                    entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
                    return new AttemptInfo { Count = 0, FirstAttempt = DateTime.UtcNow };
                });

                if (attempts.Count >= 5)
                {
                    _logger.LogWarning("Rate limit exceeded for IP: {IpAddress}", ipAddress);
                    context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.Response.WriteAsJsonAsync(new
                    {
                        message = "Too many login attempts. Please try again later.",
                        retryAfter = attempts.FirstAttempt.AddMinutes(15)
                    });
                    return;
                }

                attempts.Count++;
                _cache.Set(cacheKey, attempts);
            }

            await _next(context);
        }

        private class AttemptInfo
        {
            public int Count { get; set; }
            public DateTime FirstAttempt { get; set; }
        }
    }
}