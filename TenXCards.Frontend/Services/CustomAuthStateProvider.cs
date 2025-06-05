using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;

namespace TenXCards.Frontend.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationState _anonymous;

        public CustomAuthStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
            _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrWhiteSpace(token))
                return _anonymous;

            var claims = ParseClaimsFromJwt(token);
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            return new AuthenticationState(user);
        }

        public void NotifyUserLogin(ClaimsPrincipal userClaimsPrincipal)
        {
            var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(userClaimsPrincipal)));
            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(_anonymous);
            NotifyAuthenticationStateChanged(authState);
        }

        private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);            
            return token.Claims;
        }
    }
}
