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
    private const string TokenKey = "authToken";
    private const string UserKey = "currentUser";
    //private readonly IJSRuntime _jsRuntime;
    private UserDto? _currentUser;
    private string? _token;
    private readonly ILocalStorageService _localStorageService;

    public event Action? AuthenticationStateChanged;

    public AuthenticationService(IJSRuntime jsRuntime, ILocalStorageService localStorageService)
    {
        //_jsRuntime = jsRuntime;
        _localStorageService = localStorageService;
    }

    public async Task<bool> LoginAsync(LoginResultDto loginResult)
    {
        try
        {
            await _localStorageService.SetItem(TokenKey, loginResult);
            //await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, loginResult.Token);
            //await _jsRuntime.InvokeVoidAsync("localStorage.setItem", UserKey, 
                //System.Text.Json.JsonSerializer.Serialize(loginResult.User));
            
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
        //await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
        //await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", UserKey);
        
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
            //_token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", TokenKey);
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
            //var userJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", UserKey);
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