using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NSubstitute;
using NSubstitute.Core;
using TenXCards.API.Configuration;
using TenXCards.API.Controllers;
using TenXCards.API.Data;
using TenXCards.API.Data.Models;
using TenXCards.API.Models;
using TenXCards.API.Services;
using TenXCards.API.Tests.Helpers;

namespace TenXCards.API.Tests.Services;

public class CardServiceTests
{
    [Fact]
    public void CardService_ShouldImplementICardService()
    {
        // Assert that CardService implements ICardService
        typeof(CardService).Should().Implement<ICardService>();
    }
    
    [Theory, AutoMoqData]
    public async Task GetCardById_WhenCardExists_ShouldReturnCard(
        int cardId,
        int userId,
        [Frozen] IMemoryCache cache,
        Mock<ILogger<CardService>> loggerMock,
        Mock<IOptions<CacheOptions>> cacheOptionsMock,
        Mock<IOpenRouterService> openRouterServiceMock)
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        using var dbContext = new ApplicationDbContext(options);

        var card = new Card
        {
            Id = cardId,
            UserId = userId,
            OriginalContentId = 1,
            Front = "Test Front",
            Back = "Test Back",
            GeneratedBy = "TestGenerator",
            CreatedAt = DateTime.UtcNow
            // Fill other properties as needed
        };

        dbContext.Cards.Add(card);
        dbContext.SaveChanges();

        var sut = new CardService(
            dbContext,
            loggerMock.Object,
            cache,
            cacheOptionsMock.Object,
            openRouterServiceMock.Object);

        // Act



        var result = await sut.GetCardByIdAsync(cardId, userId);
        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(cardId);        

        //// Assert
        //result.Should().NotBeNull();
        //result.Id.Should().Be(cardId);
        //result.Front.Should().BeEquivalentTo(expectedCard.Front);
        //result.Back.Should().BeEquivalentTo(expectedCard.Back);
        //result.GeneratedBy.Should().BeEquivalentTo(expectedCard.GeneratedBy);
        //result.CreatedAt.Should().Be(expectedCard.CreatedAt);
    }
}
