using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using TenXCards.API.Configuration;
using TenXCards.API.Data;
using TenXCards.API.Exceptions;
using TenXCards.API.Models;
using TenXCards.API.Models.OpenRouter;
using TenXCards.API.Services;

namespace TenXCards.API.Tests.Services;

public class CardServiceTests
{
    private readonly Fixture _fixture;
    private readonly ILogger<CardService> _loggerMock;
    private readonly IMemoryCache _cacheMock;
    private readonly IOptions<CacheOptions> _cacheOptionsMock;
    private readonly IOpenRouterService _openRouterServiceMock;

    public CardServiceTests()
    {
        _fixture = new Fixture();
        _loggerMock = Substitute.For<ILogger<CardService>>();
        _cacheMock = Substitute.For<IMemoryCache>();
        _cacheOptionsMock = Microsoft.Extensions.Options.Options.Create(new CacheOptions());
        _openRouterServiceMock = Substitute.For<IOpenRouterService>();
    }
    
    [Fact]
    public void CardService_ShouldImplementICardService()
    {
        // Assert that CardService implements ICardService
        typeof(CardService).Should().Implement<ICardService>();
    }

    #region GenerateCardAsync Tests

    [Fact]
    public async Task GenerateCardAsync_WhenOriginalContentIsEmpty_ShouldThrowValidationException()
    {
        // Arrange
        var sut = CreateCardService();
        var command = new GenerateCardCommand { OriginalContent = string.Empty };
        var userId = _fixture.Create<int>();
        
        // Act
        Func<Task> act = async () => await sut.GenerateCardAsync(command, userId);
        
        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Original content cannot be empty");
    }
    
    [Theory]
    [InlineData(999)]  // Too short
    [InlineData(10001)] // Too long
    public async Task GenerateCardAsync_WhenOriginalContentOutsideAllowedRange_ShouldThrowValidationException(int contentLength)
    {
        // Arrange
        var sut = CreateCardService();
        var command = new GenerateCardCommand { OriginalContent = new string('a', contentLength) };
        var userId = _fixture.Create<int>();
        
        // Act
        Func<Task> act = async () => await sut.GenerateCardAsync(command, userId);
        
        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Original content must be between 1000 and 10000 characters");
    }
    
    [Fact]
    public async Task GenerateCardAsync_WhenOpenRouterServiceThrowsHttpRequestException_ShouldThrowAIGenerationException()
    {
        // Arrange
        var originalContent = new string('a', 1000);
        var command = new GenerateCardCommand { OriginalContent = originalContent };
        var userId = _fixture.Create<int>();
          _openRouterServiceMock
            .SendMessageAsync(Arg.Any<Prompt>())
            .ThrowsAsyncForAnyArgs(new HttpRequestException("Connection error"));
        
        var sut = CreateCardService();
        
        // Act
        Func<Task> act = async () => await sut.GenerateCardAsync(command, userId);
        
        // Assert
        await act.Should().ThrowAsync<AIGenerationException>()
            .WithMessage("Failed to communicate with AI service");
    }
    
    [Fact]
    public async Task GenerateCardAsync_WhenOpenRouterServiceThrowsTaskCanceledException_ShouldThrowAIGenerationException()
    {
        // Arrange
        var originalContent = new string('a', 1000);
        var command = new GenerateCardCommand { OriginalContent = originalContent };
        var userId = _fixture.Create<int>();
        
        _openRouterServiceMock
            .SendMessageAsync(Arg.Any<Prompt>())
            .ThrowsAsync(new TaskCanceledException("Request timeout"));
        
        var sut = CreateCardService();
        
        // Act
        Func<Task> act = async () => await sut.GenerateCardAsync(command, userId);
        
        // Assert
        await act.Should().ThrowAsync<AIGenerationException>()
            .WithMessage("Failed to communicate with AI service");
    }
    
    [Fact]
    public async Task GenerateCardAsync_WhenAIResponseIsInvalidJson_ShouldThrowAIGenerationException()
    {
        // Arrange
        var originalContent = new string('a', 1000);
        var command = new GenerateCardCommand { OriginalContent = originalContent };
        var userId = _fixture.Create<int>();
        
        _openRouterServiceMock
            .SendMessageAsync(Arg.Any<Prompt>())
            .Returns("Invalid JSON response");
        
        var sut = CreateCardService();
        
        // Act
        Func<Task> act = async () => await sut.GenerateCardAsync(command, userId);
        
        // Assert
        await act.Should().ThrowAsync<JsonException>();
    }
    
    [Fact]
    public async Task GenerateCardAsync_WhenAIResponseIsEmpty_ShouldThrowAIGenerationException()
    {
        // Arrange
        var originalContent = new string('a', 1000);
        var command = new GenerateCardCommand { OriginalContent = originalContent };
        var userId = _fixture.Create<int>();
        
        _openRouterServiceMock
            .SendMessageAsync(Arg.Any<Prompt>())
            .Returns("[]");
        
        var sut = CreateCardService();
        
        // Act
        Func<Task> act = async () => await sut.GenerateCardAsync(command, userId);
        
        // Assert
        await act.Should().ThrowAsync<AIGenerationException>()
            .WithMessage("AI generated invalid card content");
    }

    [Fact]
    public async Task GenerateCardAsync_WhenSuccessful_ShouldReturnValidCardDtos()
    {
        // Arrange
        var originalContent = "This is test content with sufficient length " + new string('a', 990);
        var command = new GenerateCardCommand { OriginalContent = originalContent };
        var userId = _fixture.Create<int>();
        
        // Prepare AI response
        var aiResponse = JsonSerializer.Serialize(new[] 
        {
            new { Front = "Test Front 1", Back = "Test Back 1" },
            new { Front = "Test Front 2", Back = "Test Back 2" }
        });
        
        _openRouterServiceMock
            .SendMessageAsync(Arg.Any<Prompt>())
            .Returns(aiResponse);
        
        var sut = CreateCardService();
        
        // Act
        var result = await sut.GenerateCardAsync(command, userId);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result[0].Front.Should().Be("Test Front 1");
        result[0].Back.Should().Be("Test Back 1");
        result[1].Front.Should().Be("Test Front 2");
        result[1].Back.Should().Be("Test Back 2");
    }

    #endregion

    #region Helper Methods

    private CardService CreateCardService()
    {
        return new CardService(
            CreateDbContext(),
            _loggerMock,
            _cacheMock,
            _cacheOptionsMock,
            _openRouterServiceMock);
    }

    private ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        
        var dbContext = new ApplicationDbContext(options);
        return dbContext;
    }
    
    #endregion
}
