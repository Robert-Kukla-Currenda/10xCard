using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Playwright;

namespace TenXCards.E2E.Tests;

[TestClass]
public sealed class BasicNavigationTests : PlaywrightTest
{
    [TestMethod]
    public async Task ApplicationShouldLoad()
    {
        // Arrange
        await using var browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        var page = await browser.NewPageAsync();
        
        // Act
        // Navigate to the application URL (replace with actual URL when deployed)
        await page.GotoAsync("https://localhost:7069");
        
        // Assert
        // Check that the page loaded successfully
        var title = await page.TitleAsync();
        title.Should().NotBeNullOrEmpty("The page should have a title");
        
        // Take a screenshot for debugging
        await page.ScreenshotAsync(new PageScreenshotOptions 
        { 
            Path = "screenshot-home.png" 
        });
        
        // Clean up
        await browser.CloseAsync();
    }
}
