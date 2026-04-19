using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Application.Features.Messaging.Commands;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;
using Xunit;

namespace PortfolioApi.Tests.Messaging;

public class CreateMessageCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<ILogger<CreateMessageCommandHandler>> _loggerMock;
    private readonly CreateMessageCommandHandler _handler;

    public CreateMessageCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _loggerMock = new Mock<ILogger<CreateMessageCommandHandler>>();
        
        var mockMessageSet = new Mock<Microsoft.EntityFrameworkCore.DbSet<Message>>();
        
        _contextMock.Setup(x => x.Messages).Returns(mockMessageSet.Object);
        _contextMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        _handler = new CreateMessageCommandHandler(_contextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var command = new CreateMessageCommand(
            "John Doe",
            "john@example.com",
            "Test Subject",
            "This is a test message",
            "127.0.0.1",
            "Mozilla/5.0");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Handle_WithValidData_CallsSaveChanges()
    {
        // Arrange
        var command = new CreateMessageCommand(
            "John Doe",
            "john@example.com",
            "Test Subject",
            "This is a test message",
            null,
            null);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("", "john@example.com", "Subject", "Content")]
    [InlineData("John", "", "Subject", "Content")]
    [InlineData("John", "john@example.com", "Subject", "")]
    public async Task Handle_WithMissingRequiredFields_CreatesMessage(string name, string email, string subject, string content)
    {
        // Arrange
        var command = new CreateMessageCommand(name, email, subject, content, null, null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert - Handler should still create the message (no validation in handler)
        result.Success.Should().BeTrue();
    }
}