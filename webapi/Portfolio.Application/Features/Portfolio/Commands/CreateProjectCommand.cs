using MediatR;
using PortfolioApi.Application.DTOs;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;
using System.Text.Json;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public record CreateProjectCommand(Project Project) : IRequest<ApiResponse<Project>>;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ApiResponse<Project>>
{
    private readonly IApplicationDbContext _context;

    public CreateProjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse<Project>> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = new Project
        {
            Title = request.Project.Title,
            Year = request.Project.Year,
            Category = request.Project.Category,
            Description = request.Project.Description,
            Stack = request.Project.Stack,
            Image = request.Project.Image,
            LiveUrl = request.Project.LiveUrl,
            GithubUrl = request.Project.GithubUrl,
            Status = request.Project.Status,
            Color = request.Project.Color,
            IsFeatured = request.Project.IsFeatured,
            DisplayOrder = request.Project.DisplayOrder
        };
        
        _context.Projects.Add(project);
        await _context.SaveChangesAsync(cancellationToken);
        
        return new ApiResponse<Project> { Success = true, Message = "Project created", Data = project };
    }
}
