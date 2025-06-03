using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TenXCards.API.Models;
using TenXCards.Frontend.Configuration;
using TenXCards.Frontend.Services.Handlers;

namespace TenXCards.Frontend.Services;

public interface IAuthenticationService
{
    Task<bool> LoginAsync(LoginResultDto loginResult);
    Task LogoutAsync();
    Task<UserDto?> GetCurrentUserAsync();
    Task<ClaimsPrincipal> GetCurrentUserClaimsFromTokenAsync(string token);


    event Action? AuthenticationStateChanged;
}

public class AuthenticationService : IAuthenticationService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private UserDto? _currentUser;
    private string? _currentToken;
    private const string SessionKey = "UserSession";

    public event Action? AuthenticationStateChanged;

    public AuthenticationService(
        IOptions<APIConfiguration> apiConfiguration,
        IHttpContextAccessor httpContextAccessor)
    {
        _issuer = apiConfiguration.Value.Jwt.Issuer;
        _audience = apiConfiguration.Value.Jwt.Audience;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<bool> LoginAsync(LoginResultDto loginResult)
    {
        if (loginResult == null || string.IsNullOrEmpty(loginResult.Token))
            return Task.FromResult(false);

        var userClaims = ValidateAndParseToken(loginResult.Token);
        
        
        //var handler = new JwtSecurityTokenHandler();
        //var jwtToken = handler.ReadJwtToken(loginResult.Token);

        //// Store user info
        //_currentUser = new UserDto
        //{
        //    //Id = int.Parse(jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),
        //    //Email = jwtToken.Claims.First(c => c.Type == ClaimTypes.Email).Value
        //    Email = "ddd"
        //};

        //// Store token for future use
        //_currentToken = loginResult.Token;

        _httpContextAccessor.HttpContext.Response.Cookies.Append("apiToken", loginResult.Token);

        AuthenticationStateChanged?.Invoke();
        return Task.FromResult(true);
    }

    public Task LogoutAsync()
    {        
        _httpContextAccessor.HttpContext.Response.Cookies.Delete("apiToken");

        AuthenticationStateChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task<ClaimsPrincipal> GetCurrentUserClaimsFromTokenAsync(string token)
    {
        try
        {
            return Task.FromResult(ValidateAndParseToken(token));
        }
        catch
        {
            return Task.FromResult<ClaimsPrincipal>(null);
        }        
    }

    public Task<UserDto?> GetCurrentUserAsync()
    {
        var s = _httpContextAccessor.HttpContext?.Session;
        if (_currentUser != null)
            return Task.FromResult<UserDto?>(_currentUser);

        string? token = null;

        try
        {
            token = _currentToken ?? _httpContextAccessor.HttpContext?.Session.GetString(SessionKey);
        }
        catch (InvalidOperationException)
        {
            token = _currentToken;
        }

        if (string.IsNullOrEmpty(token))
            return Task.FromResult<UserDto?>(null);

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        _currentUser = new UserDto
        {
            Id = int.Parse(jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value),
            Email = jwtToken.Claims.First(c => c.Type == ClaimTypes.Email).Value
        };

        return Task.FromResult<UserDto?>(_currentUser);
    }

    private ClaimsPrincipal ValidateAndParseToken(string token)
    {
        var tokenUserClaims = JwtValidationHelper.ValidateJwtToken(token, _issuer, _audience);
        
        var firstName = tokenUserClaims.Claims.First(c => c.Type == "firstName").Value;
        var lastName = tokenUserClaims.Claims.First(c => c.Type == "lastName").Value;

        var claimsIdentity = new ClaimsIdentity(tokenUserClaims.Claims, "jwt");
        claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, $"{firstName} {lastName}"));
        
        return new ClaimsPrincipal(claimsIdentity);
    }
}