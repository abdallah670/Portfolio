using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;
using System.Text.Json;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public record CreateProjectCommand(Project Project) : IRequest<ApiResponse<Project>>;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ApiResponse<Project>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<CreateProjectCommandHandler> _logger;
    public CreateProjectCommandHandler(IApplicationDbContext context, ILogger<CreateProjectCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<Project>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        try{
        _logger.LogInformation("Creating project: {ProjectTitle}", request.Project.Title);
        var project = new Project
        {
            Title = request.Project.Title,
            Year = request.Project.Year,
            Category = request.Project.Category,
            Description = request.Project.Description,
            Stack = request.Project.Stack,
            Image = request.Project.Image,
            linkedinUrl = request.Project.linkedinUrl,
            GithubUrl = request.Project.GithubUrl,
            Status = request.Project.Status,
            Color = request.Project.Color,
            IsFeatured = request.Project.IsFeatured,
            DisplayOrder = request.Project.DisplayOrder
        };
        
        _context.Projects.Add(project);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Project created: {ProjectId}", project.Id);
        return new ApiResponse<Project> { Success = true, Message = "Project created", Data = project };}
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error creating project: {ProjectData}", JsonSerializer.Serialize(request.Project));
            return new ApiResponse<Project> { Success = false, Message = "Error creating project" };
        }
    }
}
