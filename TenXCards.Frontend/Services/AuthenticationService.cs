using System.Security.Claims;
using Microsoft.JSInterop;
using TenXCards.API.Models;

namespace TenXCards.Frontend.Services;

public interface IAuthenticationService
{
    Task<bool> LoginAsync(LoginResultDto loginResult);
    Task LogoutAsync();
    Task<string?> GetTokenAsync();
    Task<UserDto?> GetCurrentUserAsync();
    
    event Action? AuthenticationStateChanged;
}

public class AuthenticationService : IAuthenticationService
{
    private UserDto? _currentUser;
    private string? _token;

    public event Action? AuthenticationStateChanged;

    public AuthenticationService()
    {        
    }

    public async Task<bool> LoginAsync(LoginResultDto loginResult)
    {
        try
        {            
            _token = loginResult.Token;
            _currentUser = loginResult.User;
            
            AuthenticationStateChanged?.Invoke();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task LogoutAsync()
    {   
        _token = null;
        _currentUser = null;
        
        AuthenticationStateChanged?.Invoke();
    }

    public async Task<string?> GetTokenAsync()
    {
        if (_token != null)
            return _token;

        try
        {
            //load token from session
            return _token;
        }
        catch
        {
            return null;
        }
    }

    public async Task<UserDto?> GetCurrentUserAsync()
    {
        if (_currentUser != null)
            return _currentUser;

        try
        {
            var userJson = "";
            if (string.IsNullOrEmpty(userJson))
                return null;

            _currentUser = System.Text.Json.JsonSerializer.Deserialize<UserDto>(userJson);
            return _currentUser;
        }
        catch
        {
            return null;
        }
    }
}