using FluentAssertions;
using Microsoft.Playwright;
using TenXCards.E2E.Tests.Helpers;
using TenXCards.E2E.Tests.PageObjects;

namespace TenXCards.E2E.Tests;

[TestClass]
public class AuthenticationTests : PlaywrightTest
{
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private IPage? _page;
    private string _baseUrl = string.Empty;

    [TestInitialize]
    public override void TestInitialize()
    {
        base.TestInitialize();
        SetupBrowserAsync().GetAwaiter().GetResult();
    }

    [TestCleanup]
    public override void TestCleanup()
    {
        base.TestCleanup();
        CleanupBrowserAsync().GetAwaiter().GetResult();
    }

    [TestMethod]
    public async Task Login_WithValidCredentials_ShouldRedirectToHomePageAndLogout()
    {
        // Arrange
        // Ensure page is initialized
        if (_page == null)
            throw new InvalidOperationException("Page is not initialized");

        // Navigate to the application URL
        await _page.GotoAsync($"{_baseUrl}/login");

        // Wait for the page to be fully loaded
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Initialize page objects
        var loginPage = new LoginPage(_page);
        var signedInHomePage = new SignedInHomePage(_page);

        // Take a screenshot for reference
        await loginPage.TakeScreenshotAsync("authentication-login-page");        

        // Verify login page loaded
        var isLoginPage = await loginPage.IsPageLoadedAsync();
        isLoginPage.Should().Be(true, "Login page should be loaded before performing login");        
        
        // Act - perform login
        await loginPage.LoginAsync(TestSettings.Users.Valid.Email, TestSettings.Users.Valid.Password);

        // Niezbut elegancie, ale nie WaitForLoad nie dzia�a w tym przypadku poprawnie
        Thread.Sleep(2000);

        // Take a screenshot for reference
        await signedInHomePage.TakeScreenshotAsync("authentication-home-page");        

        // Assert - should be redirected to home page
        var isLoggedHomePage = await signedInHomePage.IsPageLoadedAsync();
        isLoggedHomePage.Should().BeTrue("After successful login, user should be redirected to the home page");

        Thread.Sleep(3000);

        // Act - Logout from application
        await signedInHomePage.LogoutAsync();
        
        // Wait for logout process to complete and UI to update
        Thread.Sleep(2000);

        // Initialize signed out home page object
        var signedOutHomePage = new SignedOutHomePage(_page);

        // Take a screenshot after logout
        await signedOutHomePage.TakeScreenshotAsync("authentication-signedout-page");        
        
        // Assert - should be logged out and redirected to home page
        var signedOutMessage = await signedOutHomePage.GetSnackbarMessageAsync();
        signedOutMessage.Should().Be("Zostałeś wylogowany.", "After successful logout, user should be logged out and see login option");
    }
    
    [TestMethod]
    public async Task Login_WithInvalidCredentials_ShouldShowErrorMessage()
    {
        // Arrange
        // Ensure page is initialized
        if (_page == null)
            throw new InvalidOperationException("Page is not initialized");

        // Navigate to the application URL
        await _page.GotoAsync($"{_baseUrl}/login");

        // Wait for the page to be fully loaded
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Initialize page objects
        var loginPage = new LoginPage(_page);

        // Take a screenshot for reference
        await loginPage.TakeScreenshotAsync("authentication-login-page-load");

        // Verify login page loaded
        var loginResult = await loginPage.IsPageLoadedAsync();
        loginResult.Should().Be(true, "Login page should be loaded before performing login");        
        
        // Act - perform login with invalid credentials
        await loginPage.LoginAsync(TestSettings.Users.Invalid.Email, TestSettings.Users.Invalid.Password);

        // Niezbut elegancie, ale nie WaitForLoad nie dzia�a w tym przypadku poprawnie
        Thread.Sleep(2000);

        // Take a screenshot for reference
        await loginPage.TakeScreenshotAsync("authentication-login-page-invalid-credentials");

        // Assert - should show error message
        var errorMessage = await loginPage.GetErrorMessageAsync();
        errorMessage.Should().Be("Nieprawidłowy email lub hasło.");        
    }

    private async Task SetupBrowserAsync()
    {
        _baseUrl = TestSettings.BaseUrl;

        // Setup browser with configured options
        _browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = TestSettings.Headless,
            SlowMo = TestSettings.SlowMo
        });

        // Create a new browser context
        _context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize
            {
                Width = TestSettings.ViewportWidth,
                Height = TestSettings.ViewportHeight
            }
        });

        // Create a new page
        _page = await _context.NewPageAsync();

        // Set timeouts
        if (_page != null)
        {
            _page.SetDefaultNavigationTimeout(TestSettings.NavigationTimeout);
            _page.SetDefaultTimeout(TestSettings.ActionTimeout);
        }
    }

    private async Task CleanupBrowserAsync()
    {
        if (_context != null)
            await _context.CloseAsync();

        if (_browser != null)
            await _browser.CloseAsync();
    }
}
