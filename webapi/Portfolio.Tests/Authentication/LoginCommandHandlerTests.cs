using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Application.Features.Authentication.Commands;
using PortfolioApi.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using Xunit;

namespace PortfolioApi.Tests.Authentication;

public class LoginCommandHandlerTests
{
    private readonly Mock<UserManager<AdminUser>> _userManagerMock;
    private readonly Mock<SignInManager<AdminUser>> _signInManagerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userManagerMock = new Mock<UserManager<AdminUser>>(
            Mock.Of<IUserStore<AdminUser>>(),
            null!, null!, null!, null!, null!, null!, null!, null!);
        
        _signInManagerMock = new Mock<SignInManager<AdminUser>>(
            _userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<AdminUser>>(),
            null!, null!, null!);
        
        _configurationMock = new Mock<IConfiguration>();
        
        _handler = new LoginCommandHandler(
            _userManagerMock.Object,
            _signInManagerMock.Object,
            _configurationMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsSuccess()
    {
        // Arrange
        var request = new LoginRequest { Username = "testuser", Password = "TestPassword123" };
        var command = new LoginCommand(request);
        
        var user = new AdminUser { Id = 1, UserName = "testuser" };
        
        _userManagerMock.Setup(x => x.FindByNameAsync("testuser"))
            .ReturnsAsync(user);
        
        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, "TestPassword123", false))
            .ReturnsAsync(SignInResult.Success);
        
        _configurationMock.Setup(x => x["Jwt:Secret"]).Returns("test-secret-key-that-is-at-least-32-characters-long");
        _configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("TestIssuer");
        _configurationMock.Setup(x => x["Jwt:Audience"]).Returns("TestAudience");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Handle_WithInvalidUsername_ReturnsFailure()
    {
        // Arrange
        var request = new LoginRequest { Username = "nonexistent", Password = "Password123" };
        var command = new LoginCommand(request);
        
        _userManagerMock.Setup(x => x.FindByNameAsync("nonexistent"))
            .ReturnsAsync((AdminUser?)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid credentials");
    }

    [Fact]
    public async Task Handle_WithInvalidPassword_ReturnsFailure()
    {
        // Arrange
        var request = new LoginRequest { Username = "testuser", Password = "WrongPassword" };
        var command = new LoginCommand(request);
        
        var user = new AdminUser { Id = 1, UserName = "testuser" };
        
        _userManagerMock.Setup(x => x.FindByNameAsync("testuser"))
            .ReturnsAsync(user);
        
        _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, "WrongPassword", false))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid credentials");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handle_WithEmptyUsername_ReturnsFailure(string? username)
    {
        // Arrange
        var request = new LoginRequest { Username = username!, Password = "Password123" };
        var command = new LoginCommand(request);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
    }
}