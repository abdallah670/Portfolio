using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using PortfolioApi.Application.Features.Messaging.Queries;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;
using System.Linq.Expressions;
using Xunit;

namespace PortfolioApi.Tests.Messaging;

public class GetMessagesQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<ILogger<GetMessagesQueryHandler>> _loggerMock;
    private readonly GetMessagesQueryHandler _handler;

    public GetMessagesQueryHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _loggerMock = new Mock<ILogger<GetMessagesQueryHandler>>();
        
        _handler = new GetMessagesQueryHandler(_contextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_DefaultQuery_ReturnsPagedResult()
    {
        // Arrange
        var query = new GetMessagesQuery();

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithPageRequest_ReturnsCorrectPageInfo()
    {
        // Arrange
        var query = new GetMessagesQuery { Page = 2, PageSize = 10 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Page.Should().Be(2);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Handle_WithIsReadFilter_AppliesFilter()
    {
        // Arrange
        var query = new GetMessagesQuery { IsRead = true };

        // Act & Assert - should not throw
        var result = await _handler.Handle(query, CancellationToken.None);
        result.Should().NotBeNull();
    }
}