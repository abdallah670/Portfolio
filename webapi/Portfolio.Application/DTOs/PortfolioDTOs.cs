using System.Collections.Generic;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.DTOs;

public class HeroDto
{
    public string Name { get; set; } = string.Empty;
    public string HeadlineTop { get; set; } = string.Empty;
    public string HeadlineMain { get; set; } = string.Empty;
    public string AvailabilityLabel { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
   
    public string ProfileImage { get; set; } = string.Empty;
    public List<HeroStatsDto> Stats { get; set; } = new();
}

public class HeroStatsDto
{
    public string Label { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

public class SkillCategoryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public List<SkillDto> Skills { get; set; } = new();
}

public class SkillDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
}

public class ProjectDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Year { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Stack { get; set; } = string.Empty; // JSON serialized list
    public string Image { get; set; } = string.Empty;
    public string linkedinUrl { get; set; } = string.Empty;
    public string GithubUrl { get; set; } = string.Empty;
    public string LiveUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public bool IsPublished{get;set;}
}

public class JourneyItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Period { get; set; } = string.Empty;
    public string Org { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

public class SocialLinkDto
{
    public string Label { get; set; } = string.Empty;
    public string Href { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

public class ContactDto
{
    public string Email { get; set; } = string.Empty;
    public string WhatsApp { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}

public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}

public class PortfolioConfigDto
{
    public HeroDto Hero { get; set; } = new();
    public List<SkillCategoryDto> Skills { get; set; } = new();
    public List<ProjectDto> FeaturedProjects { get; set; } = new();
    public List<ProjectDto> MoreProjects { get; set; } = new();
    public List<JourneyItemDto> Journey { get; set; } = new();
    public List<SocialLinkDto> Socials { get; set; } = new();
    public ContactDto Contact { get; set; } = new();
}

public class UpdateHeroRequest
{
    public Hero Hero { get; set; } = new();
    public List<HeroStats>? Stats { get; set; }
}
