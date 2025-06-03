using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace TenXCards.Frontend.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());
    private readonly IAuthenticationService _authService;    
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CustomAuthenticationStateProvider(IAuthenticationService authService, IHttpContextAccessor httpContextAccessor)
    {
        _authService = authService;
        _authService.AuthenticationStateChanged += AuthenticationStateChangedHandler;
        _httpContextAccessor = httpContextAccessor;
    }

    private void AuthenticationStateChangedHandler()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var isApiToken = _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue("apiToken", out var apiToken);
        if (!isApiToken || string.IsNullOrEmpty(apiToken))
        {
            return new AuthenticationState(_anonymous);
        }

        var userClaims = await _authService.GetCurrentUserClaimsFromTokenAsync(apiToken);
        return new AuthenticationState(userClaims ?? _anonymous);
    }
}