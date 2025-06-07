using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
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

    //[Theory, AutoMoqData]
    //public async Task SendMessageAsync_WithValidPrompt_WhenApiCallSucceeds_Should_ReturnContent(
    //    [Frozen] HttpClient httpClient,
    //    [Frozen] IOptions<AIServiceOptions> options,
    //    HttpResponseMessage httpResponse,
    //    string expectedContent,
    //    OpenRouterService sut)
    //{
    //    // Arrange
    //    var prompt = new Prompt
    //    {
    //        messages = new List<Message>
    //        {
    //            new() { Role = MessageRole.User, Content = "Test message" }
    //        }
    //    };

    //    // Mock successful HTTP response
    //    httpResponse.StatusCode = HttpStatusCode.OK;

    //    var jsonResponse = new
    //    {
    //        choices = new[]
    //        {
    //            new
    //            {
    //                message = new
    //                {
    //                    content = expectedContent
    //                }
    //            }
    //        }
    //    };

    //    var jsonContent = JsonSerializer.Serialize(jsonResponse);
    //    httpResponse.Content = new StringContent(jsonContent);

    //    httpClient.PostAsJsonAsync(Arg.Any<string>(), Arg.Any<object>(),
    //            Arg.Any<JsonSerializerOptions>(), Arg.Any<CancellationToken>())
    //        .Returns(httpResponse);

    //    // Act
    //    var result = await sut.SendMessageAsync(prompt);

    //    // Assert
    //    result.Should().Be(expectedContent);
    //}

    //[Theory, AutoMoqData]
    //public async Task SendMessageAsync_WhenApiReturnsNonSuccess_Should_ThrowNetworkException(
    //    [Frozen] HttpClient httpClient,
    //    //HttpResponseMessage httpResponse,
    //    OpenRouterService sut)
    //{
    //    // Arrange
    //    var prompt = new Prompt
    //    {
    //        messages = new List<Message>
    //        {
    //            new() { Role = MessageRole.User, Content = "Test message" }
    //        }
    //    };
        
    //    // Mock failed HTTP response
    //    var httpResponse = new HttpResponseMessage();
    //    httpResponse.StatusCode = HttpStatusCode.BadRequest;
    //    httpResponse.Content = new StringContent("Bad Request");

    //    httpClient.PostAsJsonAsync(Arg.Any<string>(),
    //                               Arg.Any<object>(),
    //                               Arg.Any<JsonSerializerOptions>(),
    //                               Arg.Any<CancellationToken>())
    //              .Returns(httpResponse);


    //    // Act & Assert
    //    await FluentActions.Invoking(() => sut.SendMessageAsync(prompt))
    //        .Should().ThrowAsync<NetworkException>()
    //        .WithMessage("API request failed: BadRequest");
    //}

    //[Theory, AutoMoqData]
    //public async Task SendMessageAsync_WhenApiReturnsIncompleteResponse_Should_ThrowValidationException(
    //    [Frozen] HttpClient httpClient,
    //    HttpResponseMessage httpResponse,
    //    OpenRouterService sut)
    //{
    //    // Arrange
    //    var prompt = new Prompt
    //    {
    //        messages = new List<Message>
    //        {
    //            new() { Role = MessageRole.User, Content = "Test message" }
    //        }
    //    };

    //    // Mock incomplete response
    //    httpResponse.StatusCode = HttpStatusCode.OK;
    //    var jsonResponse = new { choices = new object[] { } }; // Empty choices array
    //    var jsonContent = JsonSerializer.Serialize(jsonResponse);
    //    httpResponse.Content = new StringContent(jsonContent);

    //    httpClient.PostAsJsonAsync(Arg.Any<string>(), Arg.Any<object>(), 
    //            Arg.Any<JsonSerializerOptions>(), Arg.Any<CancellationToken>())
    //        .Returns(httpResponse);

    //    // Act & Assert
    //    await FluentActions.Invoking(() => sut.SendMessageAsync(prompt))
    //        .Should().ThrowAsync<ValidationException>()
    //        .WithMessage("Invalid response format - missing result");
    //}

    //[Theory, AutoMoqData]
    //public async Task SendMessageAsync_WhenGeneralExceptionOccurs_Should_LogAndWrapException(
    //    [Frozen] HttpClient httpClient,
    //    [Frozen] ILogger<OpenRouterService> logger,
    //    Exception originalException,
    //    OpenRouterService sut)
    //{
    //    // Arrange
    //    var prompt = new Prompt
    //    {
    //        messages = new List<Message>
    //        {
    //            new() { Role = MessageRole.User, Content = "Test message" }
    //        }
    //    };

    //    // Mock exception during HTTP request
    //    httpClient.PostAsJsonAsync(Arg.Any<string>(), Arg.Any<object>(), 
    //            Arg.Any<JsonSerializerOptions>(), Arg.Any<CancellationToken>())
    //        .Throws(originalException);

    //    // Act & Assert
    //    await FluentActions.Invoking(() => sut.SendMessageAsync(prompt))
    //        .Should().ThrowAsync<OpenRouterException>()
    //        .WithMessage("An unexpected error occurred")
    //        .Where(ex => ex.Code == "UNKNOWN_ERROR" && ex.Details?.ToString() == originalException.Message);

    //    // Verify logging occurred
    //    logger.Received(1).LogError(
    //        Arg.Is<Exception>(e => e == originalException),
    //        Arg.Is<string>(s => s == "Unexpected error during message processing"));
    //}
}
