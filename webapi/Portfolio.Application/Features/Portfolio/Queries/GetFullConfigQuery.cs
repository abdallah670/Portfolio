using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;
using System.Text.Json;

namespace PortfolioApi.Application.Features.Portfolio.Queries;

public record GetFullConfigQuery() : IRequest<PortfolioConfigDto>;

public class GetFullConfigQueryHandler : IRequestHandler<GetFullConfigQuery, PortfolioConfigDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<GetFullConfigQueryHandler> _logger;

    public GetFullConfigQueryHandler(IApplicationDbContext context, ILogger<GetFullConfigQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PortfolioConfigDto> Handle(GetFullConfigQuery request, CancellationToken cancellationToken)
    {
        var hero = await _context.Heroes.FirstOrDefaultAsync(cancellationToken) ?? new Hero();
        var heroStats = await _context.HeroStats.OrderBy(s => s.DisplayOrder).ToListAsync(cancellationToken);
        
        var skillCategories = await _context.SkillCategories
            .Include(c => c.Skills)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync(cancellationToken);
            
        var projects = await _context.Projects
            .Where(p => p.IsPublished)
            .OrderByDescending(p => p.IsFeatured)
            .ThenBy(p => p.DisplayOrder)
            .ToListAsync(cancellationToken);
        var journey = await _context.JourneyItems.OrderBy(j => j.DisplayOrder).ToListAsync(cancellationToken);
        var socials = await _context.SocialLinks.ToListAsync(cancellationToken);
        var contact = await _context.Contacts.FirstOrDefaultAsync(cancellationToken) ?? new Contact();
         _logger.LogInformation("Retrieved portfolio configuration data");
        return new PortfolioConfigDto
        {
            Hero = new HeroDto
            {
                Name = hero.Name,
                HeadlineTop = hero.HeadlineTop,
                HeadlineMain = hero.HeadlineMain,
                AvailabilityLabel = hero.AvailabilityLabel,
                Subtitle = hero.Subtitle,

                ProfileImage = hero.ProfileImage,
                Stats = heroStats.Select(s => new HeroStatsDto { Label = s.Label, Value = s.Value }).ToList()
            },
            Skills = skillCategories.Select(c => new SkillCategoryDto
            {
                Id = c.Id,
                Title = c.Title,
                Color = c.Color,
                Skills = c.Skills.Select(s => new SkillDto { Name = s.Name, Level = s.Level }).ToList()
            }).ToList(),
            FeaturedProjects = projects.Where(p => p.IsPublished&&p.IsFeatured).Select(MapProject).ToList(),
            MoreProjects = projects.Where(p => !p.IsFeatured&&p.IsPublished).Select(MapProject).ToList(),
            Journey = journey.Select(j => new JourneyItemDto
            {
                Id = j.Id,
                Title = j.Title,
                Period = j.Period,
                Org = j.Org,
                Description = j.Description,
                DisplayOrder = j.DisplayOrder
            }).ToList(),
            Socials = socials.Select(s => new SocialLinkDto { Label = s.Label, Href = s.Href, Icon = s.Icon }).ToList(),
            Contact = new ContactDto
            {
                Email = contact.Email,
                WhatsApp = contact.WhatsApp,
                Phone = contact.Phone,
                Location = contact.Location
            }
        };
    }

    private ProjectDto MapProject(Project p)
    {
        return new ProjectDto
        {
            Id = p.Id,
            Title = p.Title,
            Year = p.Year,
            Category = p.Category,
            Description = p.Description,
            Stack = p.Stack,
            Image = p.Image,
            linkedinUrl = p.linkedinUrl,
            GithubUrl = p.GithubUrl,
            LiveUrl = p.LiveUrl,
            Status = p.Status,
            Color = p.Color,
            IsFeatured = p.IsFeatured,
            IsPublished=p.IsPublished
        };
    }
}
