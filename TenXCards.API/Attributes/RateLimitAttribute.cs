using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Claims;
using TenXCards.API.Options;

namespace TenXCards.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RateLimitAttribute : ActionFilterAttribute
    {
        private readonly string? _endpoint;
        private readonly string? _action;

        public RateLimitAttribute(string? endpoint = null, string? action = null)
        {
            _endpoint = endpoint;
            _action = action;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
            var options = context.HttpContext.RequestServices.GetRequiredService<IOptions<RateLimitOptions>>().Value;
            //var userId = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            //if (string.IsNullOrEmpty(userId))
            //{
            //    context.Result = new UnauthorizedResult();
            //    return;
            //}

            var controllerName = context.RouteData.Values["controller"]?.ToString();
            var actionName = context.RouteData.Values["action"]?.ToString();
            var endpoint = _endpoint ?? controllerName;
            var action = _action ?? actionName;

            var (windowInMinutes, maxRequests) = GetRateLimitValues(options, endpoint, action);
            var key = $"rate_limit_{endpoint}_{action}";

            var counter = await cache.GetOrCreateAsync(key, entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromMinutes(windowInMinutes));
                return Task.FromResult(0);
            });

            if (counter >= maxRequests)
            {
                var retryAfter = GetRetryAfterTime(cache, key);
                context.Result = new ObjectResult(new
                {
                    message = "Rate limit exceeded",
                    retryAfter = retryAfter.TotalSeconds
                })
                {
                    StatusCode = (int)HttpStatusCode.TooManyRequests
                };
                context.HttpContext.Response.Headers.Add("Retry-After", retryAfter.TotalSeconds.ToString());
                return;
            }

            cache.Set(key, counter + 1, TimeSpan.FromMinutes(windowInMinutes));
            await next();
        }

        private (int windowInMinutes, int maxRequests) GetRateLimitValues(
            RateLimitOptions options,
            string? endpoint,
            string? action)
        {
            // Try to get endpoint-specific configuration
            // if (endpoint != null && action != null &&
            //     options.Endpoints?.TryGetValue(endpoint, out var endpointConfig) == true &&
            //     endpointConfig.TryGetValue(action, out var actionConfig) == true)
            // {
            //     return (actionConfig.WindowInMinutes, actionConfig.MaxRequestsPerWindow);
            // }

            // Fallback to global configuration
            return (options.WindowInMinutes, options.MaxRequestsPerWindow);
        }

        private TimeSpan GetRetryAfterTime(IMemoryCache cache, string key)
        {
            var cacheEntry = cache.Get<ICacheEntry>(key);
            return cacheEntry?.AbsoluteExpiration?.Subtract(DateTime.UtcNow) ?? TimeSpan.FromMinutes(1);
        }
    }
}