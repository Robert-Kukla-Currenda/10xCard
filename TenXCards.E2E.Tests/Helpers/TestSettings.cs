using System.IO;
using Microsoft.Extensions.Configuration;

namespace TenXCards.E2E.Tests.Helpers;

/// <summary>
/// Helper class for loading E2E test settings from configuration files
/// </summary>
public static class TestSettings
{
    private static readonly IConfiguration Configuration;

    static TestSettings()
    {
        // Build configuration from testsettings.json
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("testsettings.json", optional: true)
            .AddJsonFile("testsettings.local.json", optional: true) // For local overrides
            .AddEnvironmentVariables("TENX_TEST_"); // For CI/CD overrides with prefix
            
        Configuration = builder.Build();
    }
    
    // Base URL of the application under test
    public static string BaseUrl => Configuration["TestSettings:BaseUrl"] ?? "https://localhost:7069";
    
    // Browser settings
    public static string Browser => Configuration["TestSettings:Browser"] ?? "chromium";
    public static bool Headless => bool.Parse(Configuration["TestSettings:Headless"] ?? "true");
    public static int SlowMo => int.Parse(Configuration["TestSettings:SlowMo"] ?? "0");
    
    // Viewport settings
    public static int ViewportWidth => int.Parse(Configuration["TestSettings:Viewport:Width"] ?? "1280");
    public static int ViewportHeight => int.Parse(Configuration["TestSettings:Viewport:Height"] ?? "720");
    
    // Screenshot settings
    public static string ScreenshotsPath => Configuration["TestSettings:Screenshots:Path"] ?? "./Screenshots";
    public static bool TakeScreenshotOnFailure => bool.Parse(Configuration["TestSettings:Screenshots:TakeOnFailure"] ?? "true");
    
    // Timeout settings
    public static int NavigationTimeout => int.Parse(Configuration["TestSettings:Timeouts:Navigation"] ?? "30000");
    public static int ActionTimeout => int.Parse(Configuration["TestSettings:Timeouts:Action"] ?? "10000");
    public static int ExpectTimeout => int.Parse(Configuration["TestSettings:Timeouts:Expect"] ?? "5000");
    
    // Test user credentials
    public static class Users
    {
        public static class Valid
        {
            public static string Email => Configuration["TestSettings:TestUsers:Valid:Email"] ?? "x@xx.pl";
            public static string Password => Configuration["TestSettings:TestUsers:Valid:Password"] ?? "AlaMaKota1";
        }
        
        public static class Invalid
        {
            public static string Email => Configuration["TestSettings:TestUsers:Invalid:Email"] ?? "user@example.com";
            public static string Password => Configuration["TestSettings:TestUsers:Invalid:Password"] ?? "UserPassword123!";
        }
    }
}
