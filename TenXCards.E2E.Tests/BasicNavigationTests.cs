using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Playwright;
using TenXCards.E2E.Tests.Helpers;

namespace TenXCards.E2E.Tests;

[TestClass]
public sealed class BasicNavigationTests : PlaywrightTest
{
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private IPage? _page;
    private string _baseUrl = "https://localhost:7069";

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
    public async Task ApplicationShouldLoad()
    {
        // Ensure page is initialized
        if (_page == null)
            throw new InvalidOperationException("Page is not initialized");
        
        // Act
        // Navigate to the application URL
        await _page.GotoAsync(_baseUrl);
        
        // Assert
        // Check that the page loaded successfully
        var title = await _page.TitleAsync();
        title.Should().NotBeNullOrEmpty("The page should have a title");
        
        // Take a screenshot for debugging
        await _page.ScreenshotAsync(new PageScreenshotOptions 
        { 
            Path = Path.Combine(TestSettings.ScreenshotsPath, "screenshot-home.png")
        });
    }
    
    private async Task SetupBrowserAsync()
    {
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
