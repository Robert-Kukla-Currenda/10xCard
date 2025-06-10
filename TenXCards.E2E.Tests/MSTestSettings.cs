using System.IO;
using Microsoft.Playwright;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TenXCards.E2E.Tests.Helpers;

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]

namespace TenXCards.E2E.Tests;

[TestClass]
public class MSTestSettings
{
    private static IPlaywright _playwright;
    
    public static IPlaywright Playwright => _playwright;

    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext testContext)
    {
        // Initialize Playwright
        _playwright = Microsoft.Playwright.Playwright.CreateAsync().GetAwaiter().GetResult();
        
        // Create screenshots directory if it doesn't exist
        Directory.CreateDirectory(TestSettings.ScreenshotsPath);
        
        // Logging initialization
        testContext.WriteLine($"Test run initialized with settings:");
        testContext.WriteLine($"- Base URL: {TestSettings.BaseUrl}");
        testContext.WriteLine($"- Browser: {TestSettings.Browser}");
        testContext.WriteLine($"- Headless mode: {TestSettings.Headless}");
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        _playwright?.Dispose();
    }
}

/// <summary>
/// Base class for Playwright tests using MSTest
/// </summary>
public class PlaywrightTest
{
    protected IPlaywright Playwright => MSTestSettings.Playwright;
    
    [TestInitialize]
    public virtual void TestInitialize()
    {
        // Setup for each test
    }
    
    [TestCleanup]
    public virtual void TestCleanup()
    {
        // Cleanup after each test
    }
}
