using FluentAssertions;
using PortfolioApi.Domain.Entities;
using Xunit;

namespace PortfolioApi.Tests.Domain;

public class ProjectEntityTests
{
    [Fact]
    public void Project_WithValidData_IsValid()
    {
        // Arrange & Act
        var project = new Project
        {
            Id = 1,
            Title = "Test Project",
            Year = "2024",
            Category = "Web Development",
            Description = "A test project description",
            Stack = "[\"Angular\", \".NET\"]",
            Image = "/uploads/projects/test.jpg",
            GithubUrl = "https://github.com/test",
            LiveUrl = "https://test.com",
            Status = "Completed",
            Color = "#FF5733",
            IsFeatured = true,
            DisplayOrder = 1,
            IsPublished = true,
            ViewsCount = 100
        };

        // Assert
        project.Should().NotBeNull();
        project.Id.Should().Be(1);
        project.Title.Should().Be("Test Project");
    }

    [Fact]
    public void Project_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var project = new Project();

        // Assert
        project.Id.Should().Be(0);
        project.Title.Should().Be(string.Empty);
        project.IsFeatured.Should().BeFalse();
        project.IsPublished.Should().BeTrue();
        project.DisplayOrder.Should().Be(0);
        project.ViewsCount.Should().Be(0);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Project_WithEmptyTitle_ShouldAllow(string? title)
    {
        // Arrange & Act
        var project = new Project { Title = title! };

        // Assert - Entity allows empty title (validation should be at application layer)
        project.Title.Should().Be(title);
    }

    [Fact]
    public void Project_WithLongStack_CanStoreJson()
    {
        // Arrange
        var stackJson = "[\"C#\", \"Angular\", \"SQL Server\", \"Azure\", \"Docker\", \"Kubernetes\", \"CI/CD\"]";

        // Act
        var project = new Project { Stack = stackJson };

        // Assert
        project.Stack.Should().Be(stackJson);
    }

    [Fact]
    public void Project_CreatedAt_DefaultsToUtcNow()
    {
        // Arrange & Act
        var project = new Project();

        // Assert
        project.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }
}

public class MessageEntityTests
{
    [Fact]
    public void Message_WithValidData_IsValid()
    {
        // Arrange & Act
        var message = new Message
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            Subject = "Test Subject",
            Content = "This is a test message content",
            IpAddress = "127.0.0.1",
            UserAgent = "Mozilla/5.0",
            IsRead = false,
            IsReplied = false
        };

        // Assert
        message.Should().NotBeNull();
        message.Name.Should().Be("John Doe");
        message.Email.Should().Be("john@example.com");
        message.IsRead.Should().BeFalse();
    }

    [Fact]
    public void Message_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var message = new Message();

        // Assert
        message.Id.Should().Be(0);
        message.Name.Should().Be(string.Empty);
        message.Email.Should().Be(string.Empty);
        message.IsRead.Should().BeFalse();
        message.IsReplied.Should().BeFalse();
    }

    [Fact]
    public void Message_CreatedAt_DefaultsToUtcNow()
    {
        // Arrange & Act
        var message = new Message();

        // Assert
        message.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Message_WithNullOptionalFields_ShouldAllow()
    {
        // Arrange & Act
        var message = new Message
        {
            Name = "John",
            Email = "john@test.com",
            Subject = null!,
            Content = "Test",
            IpAddress = null,
            UserAgent = null
        };

        // Assert
        message.Subject.Should().BeNull();
        message.IpAddress.Should().BeNull();
        message.UserAgent.Should().BeNull();
    }
}

public class SkillEntityTests
{
    [Fact]
    public void Skill_WithValidData_IsValid()
    {
        // Arrange & Act
        var skill = new Skill
        {
            Id = 1,
            Name = "C#",
            Level = 8,
            CategoryId = 1
        };

        // Assert
        skill.Should().NotBeNull();
        skill.Name.Should().Be("C#");
        skill.Level.Should().Be(8);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public void Skill_WithValidLevelRanges_ShouldAllow(int level)
    {
        // Arrange & Act
        var skill = new Skill { Name = "Test", Level = level };

        // Assert
        skill.Level.Should().Be(level);
    }
}

public class HeroEntityTests
{
    [Fact]
    public void Hero_WithValidData_IsValid()
    {
        // Arrange & Act
        var hero = new Hero
        {
            Id = 1,
            Name = "John Doe",
            HeadlineTop = "Welcome to",
            HeadlineMain = "My Portfolio",
            Subtitle = "Full Stack Developer",
            AvailabilityLabel = "Available for work",
            ProfileImage = "/uploads/profile.jpg"
        };

        // Assert
        hero.Should().NotBeNull();
        hero.Name.Should().Be("John Doe");
    }

    [Fact]
    public void Hero_DefaultValues_AreCorrect()
    {
        // Arrange & Act
        var hero = new Hero();

        // Assert
        hero.Name.Should().Be(string.Empty);
    }
}