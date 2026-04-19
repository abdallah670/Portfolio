using MediatR;
using Microsoft.EntityFrameworkCore;
using PortfolioApi.Application.Interfaces;

namespace PortfolioApi.Application.Features.Portfolio.Queries;

public record GetProfileImageQuery() : IRequest<string>;

public class GetProfileImageQueryHandler : IRequestHandler<GetProfileImageQuery, string>
{
    private readonly IApplicationDbContext _context;

    public GetProfileImageQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(GetProfileImageQuery request, CancellationToken cancellationToken)
    {
        var hero = await _context.Heroes.FirstOrDefaultAsync(cancellationToken);
        return hero?.ProfileImage ?? "uploads/profile-image/Meno.png";
    }
}
