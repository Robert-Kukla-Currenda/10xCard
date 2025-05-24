using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace TenXCards.Frontend.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    //private readonly IAuthenticationService _authService;
    private readonly ISessionStorageService _sessionStorageService;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(ISessionStorageService sessionStorageService)
    {
        //_authService = authService;
        //_authService.AuthenticationStateChanged += OnAuthenticationStateChanged;
        _sessionStorageService = sessionStorageService;        
    }

    //private void OnAuthenticationStateChanged()
    //{
    //    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    //}

    //public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    //{
    //    var user = await _authService.GetCurrentUserAsync();

    //    if (user == null)
    //    {
    //        return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
    //    }

    //    var claims = new[]
    //    {
    //        new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
    //        new Claim(ClaimTypes.Email, user.Email),
    //        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
    //    };

    //    var identity = new ClaimsIdentity(claims, "jwt");
    //    return new AuthenticationState(new ClaimsPrincipal(identity));
    //}

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _sessionStorageService.GetItemAsync<string>("token");
        if (string.IsNullOrEmpty(token))
        {
            return new AuthenticationState(_anonymous);
        }

        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"); //JwtParser.ParseClaimsFromJwt(token)
        var user = new ClaimsPrincipal(identity);
        return await Task.FromResult(new AuthenticationState(user));
    }

    public void AuthenticateUser(string token)
    {
        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"); //JwtParser.ParseClaimsFromJwt(token)
        var user = new ClaimsPrincipal(identity);
        var state = new AuthenticationState(user);
        NotifyAuthenticationStateChanged(Task.FromResult(state));
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var x = new JwtSecurityTokenHandler();
        x.ReadJwtToken(jwt);

        var claims = new[] 
        {
                new Claim(ClaimTypes.Name, $"Robb Kukk"),
        //        new Claim(ClaimTypes.Email, user.Email),
        //        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
        return claims;
    }
}