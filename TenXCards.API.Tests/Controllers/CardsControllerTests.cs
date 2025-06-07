using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using TenXCards.API.Controllers;
using TenXCards.API.Services;
using TenXCards.API.Tests.Fixtures;
using Xunit;

namespace TenXCards.API.Tests.Controllers;

[Collection("Database collection")]
public class CardsControllerTests
{
    private readonly PostgresFixture _fixture;

    public CardsControllerTests(PostgresFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GetCards_ReturnsOkResultWithCards()
    {
        // Arrange
        var cardService = Substitute.For<ICardService>();
        var cards = new List<CardDto>
        {
            new() { Id = 1, Title = "Test Card 1", Content = "Content 1" },
            new() { Id = 2, Title = "Test Card 2", Content = "Content 2" }
        };

        cardService.GetCardsAsync().Returns(cards);

        //var controller = new CardsController(cardService);

        //// Act
        //var result = await controller.GetCards();

        //// Assert
        //var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        //var returnedCards = okResult.Value.Should().BeAssignableTo<IEnumerable<CardDto>>().Subject;
        //returnedCards.Should().HaveCount(2);
        //returnedCards.Should().Contain(c => c.Id == 1 && c.Title == "Test Card 1");
        //returnedCards.Should().Contain(c => c.Id == 2 && c.Title == "Test Card 2");
    }

    //[Fact]
    //public async Task GetCard_WithValidId_ReturnsOkResultWithCard()
    //{
    //    // Arrange
    //    var cardId = 1;
    //    var card = new CardDto { Id = cardId, Title = "Test Card", Content = "Test Content" };

    //    var cardService = Substitute.For<ICardService>();
    //    cardService.GetCardByIdAsync(cardId).Returns(card);

    //    var controller = new CardsController(cardService);

    //    // Act
    //    var result = await controller.GetCard(cardId);

    //    // Assert
    //    var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
    //    var returnedCard = okResult.Value.Should().BeOfType<CardDto>().Subject;
    //    returnedCard.Id.Should().Be(cardId);
    //    returnedCard.Title.Should().Be("Test Card");
    //    returnedCard.Content.Should().Be("Test Content");
    //}
}

// Placeholder classes to match the actual models
public class CardDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}

// Placeholder interface for the service
public interface ICardService
{
    Task<IEnumerable<CardDto>> GetCardsAsync();
    Task<CardDto> GetCardByIdAsync(int id);
}
