using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using Playwright.Axe;

namespace TenXCards.E2E.Tests;

[TestClass]
public class AccessibilityTests : PlaywrightTest
{
    [TestMethod]
    public async Task HomePage_ShouldBeAccessible()
    {
        // Arrange
        await using var browser = await Playwright.Chromium.LaunchAsync();
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();
        
        // Navigate to the application URL (replace with actual URL when deployed)
        await page.GotoAsync("https://localhost:5001");
        
        // Wait for the page to be fully loaded
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Act
        // Run axe accessibility scan
        var axeResults = await page.RunAxe();
        
        // Take a screenshot for reference
        await page.ScreenshotAsync(new PageScreenshotOptions 
        { 
            Path = "accessibility-home.png" 
        });
        
        // Assert
        // Verify there are no violations
        axeResults.Violations.Should().BeEmpty("The home page should not have any accessibility violations");
        
        // Clean up
        await browser.CloseAsync();
    }
    
    [TestMethod]
    public async Task LoginPage_ShouldBeAccessible()
    {
        // Arrange
        await using var browser = await Playwright.Chromium.LaunchAsync();
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();
        
        // Navigate to the login page (replace with actual URL when deployed)
        await page.GotoAsync("https://localhost:5001/login");
        
        // Wait for the page to be fully loaded
        await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Act
        // Run axe accessibility scan
        var axeResults = await page.RunAxe();
        
        // Take a screenshot for reference
        await page.ScreenshotAsync(new PageScreenshotOptions 
        { 
            Path = "accessibility-login.png" 
        });
        
        // Assert
        // Verify there are no violations
        axeResults.Violations.Should().BeEmpty("The login page should not have any accessibility violations");
        
        // Clean up
        await browser.CloseAsync();
    }
}
