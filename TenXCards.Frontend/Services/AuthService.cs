using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Components.Authorization;
using TenXCards.Frontend.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Security.Claims;
using TenXCards.Frontend.Validators;

namespace TenXCards.Frontend.Services
{
    public interface IAuthService
    {
        Task LoginAsync(LoginResultDto loginResult);
        Task LogoutAsync();
    }
    
    public class AuthService : IAuthService
    {
        private readonly IHttpService _httpService;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AuthService(
            IHttpService httpService, 
            ILocalStorageService localStorage,
            AuthenticationStateProvider authStateProvider)
        {
            _httpService = httpService;
            _localStorage = localStorage;
            _authStateProvider = authStateProvider;
        }

        public async Task LoginAsync(LoginResultDto loginResult)
        {
            var authStateProvider = (CustomAuthStateProvider)_authStateProvider;
            var token = loginResult.Token;
            var userClaims = ValidateAndParseToken(token);

            await _localStorage.SetItemAsync("authToken", loginResult.Token);
            authStateProvider.NotifyUserLogin(userClaims);
        }        
        
        public async Task LogoutAsync()
        {
            var authStateProvider = (CustomAuthStateProvider)_authStateProvider;
            _httpService.RemoveAuthorizations();
            await _localStorage.RemoveItemAsync("authToken");            
            authStateProvider.NotifyUserLogout();
        }

        private ClaimsPrincipal ValidateAndParseToken(string token)
        {
            var tokenUserClaims = JwtValidationHelper.ValidateJwtToken(token, "issuer", "audience");

            var firstName = tokenUserClaims.Claims.First(c => c.Type == "firstName").Value;
            var lastName = tokenUserClaims.Claims.First(c => c.Type == "lastName").Value;

            var claimsIdentity = new ClaimsIdentity(tokenUserClaims.Claims, "jwt");
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, $"{firstName} {lastName}"));

            return new ClaimsPrincipal(claimsIdentity);
        }
    }
}
