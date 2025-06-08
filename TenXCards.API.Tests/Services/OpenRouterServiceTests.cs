using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReturnsExtensions;
using TenXCards.API.Configuration;
using TenXCards.API.Exceptions;
using TenXCards.API.Models.OpenRouter;
using TenXCards.API.Services;
using TenXCards.API.Services.OpenRouter;
using TenXCards.API.Tests.Helpers;

namespace TenXCards.API.Tests.Services;

public class OpenRouterServiceTests
{
    private readonly Fixture _fixture;
    private readonly ILogger<OpenRouterService> _loggerMock;
    private readonly HttpClient _httpClientMock;
    private readonly IOptions<AIServiceOptions> _aiServiceOptionsMock;
    private readonly IErrorLoggingService _errorLoggingServiceMock;

    public OpenRouterServiceTests()
    {
        _fixture = new Fixture();
        _loggerMock = Substitute.For<ILogger<OpenRouterService>>();
        _httpClientMock = Substitute.For<HttpClient>();
        _aiServiceOptionsMock = Microsoft.Extensions.Options.Options.Create(
            new AIServiceOptions
            { 
                OpenRouterApiKey="test",
                OpenRouterUrl = "https://test.ai",
                OpenRouterModelName = "test",
                ModelTemperature = 0.7,
                ModelMaxTokens = 1000
            });                                
        _errorLoggingServiceMock = Substitute.For<IErrorLoggingService>();
    }

    [Fact]
    public void OpenRouterService_ShouldImplementIOpenRouterService()
    {
        // Assert that OpenRouterService implements IOpenRouterService
        typeof(OpenRouterService).Should().Implement<IOpenRouterService>();
    }

    [Theory, AutoMoqData]
    public async Task SendMessageAsync_WithEmptyPrompt_Should_ThrowValidationException(
        OpenRouterService sut)
    {
        // Arrange
        Prompt? nullPrompt = null;

        // Act & Assert
        await FluentActions.Invoking(() => sut.SendMessageAsync(nullPrompt!))
            .Should().ThrowAsync<ValidationException>()
            .WithMessage("Prompt cannot be empty");
    }

    [Theory, AutoMoqData]
    public async Task SendMessageAsync_WithEmptyMessagesList_Should_ThrowValidationException(
        OpenRouterService sut)
    {
        // Arrange
        var prompt = new Prompt { messages = new List<Message>() };

        // Act & Assert
        await FluentActions.Invoking(() => sut.SendMessageAsync(prompt))
            .Should().ThrowAsync<ValidationException>()
            .WithMessage("Prompt cannot be empty");
    }

    [Fact]
    public async Task SendMessageAsync_WithValidPrompt_WhenApiCallSucceeds_Should_ReturnContent()
    {
        // Arrange
        // Przygotowanie mocków
        var httpClient = new HttpClient(new MockHttpMessageHandler());
        
        // Przygotowanie danych testowych
        var expectedContent = "Test response content";
        var prompt = new Prompt
        {
            messages = new List<Message>
            {
                new() { Role = MessageRole.User, Content = "Test message" }
            }
        };
        
        // Mockowanie odpowiedzi HTTP
        var responseJson = @"{
            ""choices"": [
                {
                    ""message"": {
                        ""content"": """ + expectedContent + @"""
                    }
                }
            ]
        }";
        
        MockHttpMessageHandler.ResponseToReturn = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(responseJson)
        };

        // Tworzenie systemu pod testami (SUT)
        var sut = new OpenRouterService(
            _loggerMock,
            httpClient,
            _aiServiceOptionsMock,
            _errorLoggingServiceMock);
        
        // Act
        var result = await sut.SendMessageAsync(prompt);
        
        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedContent);
    }

    [Fact]
    public async Task SendMessageAsync_WhenApiReturnsNonSuccess_Should_ThrowNetworkException()
    {
        // Arrange
        // Przygotowanie mocków
        var httpClient = new HttpClient(new MockHttpMessageHandler());

        // Przygotowanie danych testowych
        var prompt = new Prompt
        {
            messages = new List<Message>
            {
                new() { Role = MessageRole.User, Content = "Test message" }
            }
        };

        MockHttpMessageHandler.ResponseToReturn = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = new StringContent("Bad Request")
        };

        // Tworzenie systemu pod testami (SUT)
        var sut = new OpenRouterService(
            _loggerMock,
            httpClient,
            _aiServiceOptionsMock,
            _errorLoggingServiceMock);        

        // Act & Assert
        await FluentActions.Invoking(() => sut.SendMessageAsync(prompt))
            .Should().ThrowAsync<NetworkException>()
            .WithMessage("API request failed: BadRequest");
    }

    [Fact]
    public async Task SendMessageAsync_WhenApiReturnsIncompleteResponse_Should_ThrowValidationException()
    {
        // Arrange
        // Przygotowanie mocków
        var httpClient = new HttpClient(new MockHttpMessageHandler());

        // Przygotowanie danych testowych
        var prompt = new Prompt
        {
            messages = new List<Message>
            {
                new() { Role = MessageRole.User, Content = "Test message" }
            }
        };

        // Mockowanie niekompletnej odpowiedzi HTTP
        var jsonResponse = new { choices = new object[] { } }; // Empty choices array
        var jsonContent = JsonSerializer.Serialize(jsonResponse);
        MockHttpMessageHandler.ResponseToReturn = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(jsonContent)
        };

        // Tworzenie systemu pod testami (SUT)
        var sut = new OpenRouterService(
            _loggerMock,
            httpClient,
            _aiServiceOptionsMock,
            _errorLoggingServiceMock);

        // Act & Assert
        await FluentActions.Invoking(() => sut.SendMessageAsync(prompt))
            .Should().ThrowAsync<OpenRouterException>()
            .WithMessage("An unexpected error occurred");
    }

    // Pomocnicza klasa do mockowania odpowiedzi HTTP
    private class MockHttpMessageHandler : HttpMessageHandler
    {
        public static HttpResponseMessage ResponseToReturn { get; set; } = new HttpResponseMessage();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(ResponseToReturn);
        }
    }
}
