using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace TenXCards.API.Tests.Helpers;

/// <summary>
/// Helper class for loading configuration values from appsettings.json or environment variables in tests
/// </summary>
public static class TestConfiguration
{
    private static IConfiguration _configuration;
    
    static TestConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Test.json", optional: true)
            .AddEnvironmentVariables();
            
        _configuration = builder.Build();
    }
    
    /// <summary>
    /// Gets a configuration value by key
    /// </summary>
    /// <typeparam name="T">The type to convert the value to</typeparam>
    /// <param name="key">The configuration key</param>
    /// <param name="defaultValue">Default value if not found</param>
    /// <returns>The configuration value</returns>
    public static T GetValue<T>(string key, T defaultValue = default)
    {
        return _configuration.GetValue<T>(key, defaultValue);
    }
    
    /// <summary>
    /// Gets a configuration section
    /// </summary>
    /// <param name="key">The section key</param>
    /// <returns>The configuration section</returns>
    public static IConfigurationSection GetSection(string key)
    {
        return _configuration.GetSection(key);
    }
    
    /// <summary>
    /// Gets a typed configuration section
    /// </summary>
    /// <typeparam name="T">The type to bind the section to</typeparam>
    /// <param name="sectionName">The section name</param>
    /// <returns>The typed configuration section</returns>
    public static T GetSection<T>(string sectionName) where T : new()
    {
        var section = _configuration.GetSection(sectionName);
        var result = new T();
        section.Bind(result);
        return result;
    }
}
