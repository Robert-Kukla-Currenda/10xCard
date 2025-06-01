using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using TenXCards.API.Data.Models;
using TenXCards.API.Models;
using TenXCards.Frontend.Services.Handlers;

namespace TenXCards.Frontend.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    //private readonly IAuthenticationService _authService;    
    private readonly ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
    private string? _jwtToken;
    private readonly string _issuer;
    private readonly string _audience;

    private ClaimsPrincipal _user;

    public CustomAuthenticationStateProvider()
    {
        //todo from configuration
        _issuer = "issuer";
        _audience = "audience";
        
        //_authService = authService;
        //_authService.AuthenticationStateChanged += OnAuthenticationStateChanged;        
    }

    public bool Login(LoginResultDto loginRequestResult)
    {       
        if (loginRequestResult == null || string.IsNullOrEmpty(loginRequestResult.Token))
            return false;
        var userClaims = ParseClaimsFromJwt(loginRequestResult.Token);
        _jwtToken = loginRequestResult.Token;
       
        if(userClaims == null || !userClaims.Identity.IsAuthenticated)
        {
            return false;
        }
        
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(userClaims)));
        //NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_user)));
        return true;
    }

    public void Logout()
    {
        _jwtToken = null;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    //private void OnAuthenticationStateChanged()
    //{
    //    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    //}

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (string.IsNullOrEmpty(_jwtToken))
        {
            return new AuthenticationState(_anonymous);
        }
        
        var user = ParseClaimsFromJwt(_jwtToken);
        return await Task.FromResult(new AuthenticationState(user));
    }

    private ClaimsPrincipal ParseClaimsFromJwt(string jwt)
    {
        return JWTValidationHelper.ValidateJwtToken(jwt, _issuer, _audience);
    }
}