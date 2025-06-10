using System.Threading.Tasks;
using Microsoft.Playwright;
using TenXCards.E2E.Tests.PageObjects;

namespace TenXCards.E2E.Tests;

[TestClass]
public class AuthenticationTests : PlaywrightTest
{
    [TestMethod]
    public async Task Login_WithValidCredentials_ShouldRedirectToDashboard()
    {
        // Arrange
        await using var browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();
        
        // Navigate to application
        await page.GotoAsync("https://localhost:5001");
        
        // Initialize page objects
        var loginPage = new LoginPage(page);
        var dashboardPage = new DashboardPage(page);
        
        // Verify login page loaded
        Assert.IsTrue(await loginPage.IsPageLoadedAsync());
        
        // Act - perform login
        await loginPage.LoginAsync("test@example.com", "Password123!");
        
        // Assert - should be redirected to dashboard
        Assert.IsTrue(await dashboardPage.IsPageLoadedAsync(), "Login should redirect to dashboard");
        
        // Clean up
        await browser.CloseAsync();
    }
    
    [TestMethod]
    public async Task Login_WithInvalidCredentials_ShouldShowErrorMessage()
    {
        // Arrange
        await using var browser = await Playwright.Chromium.LaunchAsync();
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();
        
        // Navigate to application
        await page.GotoAsync("https://localhost:5001");
        
        // Initialize page objects
        var loginPage = new LoginPage(page);
        
        // Verify login page loaded
        Assert.IsTrue(await loginPage.IsPageLoadedAsync());
        
        // Act - perform login with invalid credentials
        await loginPage.LoginAsync("invalid@example.com", "WrongPassword!");
        
        // Assert - should show error message
        var errorMessage = await loginPage.GetErrorMessageAsync();
        Assert.IsTrue(!string.IsNullOrEmpty(errorMessage), "Error message should be displayed");
        
        // Clean up
        await browser.CloseAsync();
    }
}
