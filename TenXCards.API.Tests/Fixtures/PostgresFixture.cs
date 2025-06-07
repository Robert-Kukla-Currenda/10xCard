using DotNet.Testcontainers.Builders;
using Testcontainers.PostgreSql;

namespace TenXCards.API.Tests.Fixtures;

public class PostgresFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    public string ConnectionString { get; private set; }
    
    public PostgresFixture()
    {        
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:15")
            .WithEnvironment("POSTGRES_USER", "test_user")
            .WithEnvironment("POSTGRES_PASSWORD", "test_password")
            .WithEnvironment("POSTGRES_DB", "tenxcards_test")
            .WithPortBinding(5432, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .Build();
    }
    
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        ConnectionString = $"Host=localhost;Port={_dbContainer.GetMappedPublicPort(5432)};Database=tenxcards_test;Username=test_user;Password=test_password";
    }
    
    public async Task DisposeAsync()
    {
        if (_dbContainer != null)
        {
            await _dbContainer.StopAsync();
            await _dbContainer.DisposeAsync();
        }
    }
}

[CollectionDefinition("Database collection")]
public class DatabaseCollection : ICollectionFixture<PostgresFixture>
{
    // This class has no code, and is never created. Its purpose is to be the place to apply
    // [CollectionDefinition] and all the ICollectionFixture<> interfaces.
}
