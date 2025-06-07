using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TenXCards.API.Data;

namespace TenXCards.API.Tests.Fixtures;

/// <summary>
/// Class for creating a clean, isolated database context for testing
/// </summary>
public static class DbContextFactory
{
    /// <summary>
    /// Creates a new database context with an in-memory database for tests that don't require a real PostgreSQL instance
    /// </summary>
    public static ApplicationDbContext CreateInMemoryDbContext(string dbName = null)
    {
        dbName ??= Guid.NewGuid().ToString();
        
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
            
        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
    
    /// <summary>
    /// Creates a new database context with a real PostgreSQL database for integration tests
    /// </summary>
    public static ApplicationDbContext CreatePostgresDbContext(string connectionString)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(connectionString)
            .Options;
            
        var context = new ApplicationDbContext(options);
        context.Database.Migrate();
        return context;
    }
}
