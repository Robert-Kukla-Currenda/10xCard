using FluentAssertions;
using Microsoft.Playwright;
using Playwright.Axe;
using TenXCards.E2E.Tests.Helpers;

namespace TenXCards.E2E.Tests;

[TestClass]
public class AccessibilityTests : PlaywrightTest
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
    public async Task HomePage_ShouldBeAccessible()
    {
        // Ensure page is initialized
        if (_page == null)
            throw new InvalidOperationException("Page is not initialized");

        // Navigate to the application URL
        await _page.GotoAsync(_baseUrl);

        // Wait for the page to be fully loaded
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Act
        // Run axe accessibility scan
        var axeResults = await _page.RunAxe();

        // Take a screenshot for reference
        await _page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = Path.Combine(TestSettings.ScreenshotsPath, "accessibility-home.png")
        });

        // Assert
        // Verify there are no violations
        axeResults.Violations.Should().BeEmpty("The home page should not have any accessibility violations");
    }

    [TestMethod]
    public async Task LoginPage_ShouldBeAccessible()
    {
        // Ensure page is initialized
        if (_page == null)
            throw new InvalidOperationException("Page is not initialized");

        // Navigate to the login page
        await _page.GotoAsync($"{_baseUrl}/login");

        // Wait for the page to be fully loaded
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

        // Act
        // Run axe accessibility scan
        var axeResults = await _page.RunAxe();

        // Take a screenshot for reference
        await _page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = Path.Combine(TestSettings.ScreenshotsPath, "accessibility-login.png")
        });

        // Assert
        // Verify there are no violations
        axeResults.Violations.Should().BeEmpty("The login page should not have any accessibility violations");
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
