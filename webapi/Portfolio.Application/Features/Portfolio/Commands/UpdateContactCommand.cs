using MediatR;
using Microsoft.Extensions.Logging;
using PortfolioApi.Application.Interfaces;
using PortfolioApi.Domain.Entities;

namespace PortfolioApi.Application.Features.Portfolio.Commands;

public class UpdateContactCommand : IRequest<Contact>
{
    public Contact Contact { get; set; } = null!;
}

public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand, Contact>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateContactCommandHandler> _logger;

    public UpdateContactCommandHandler(IApplicationDbContext context, ILogger<UpdateContactCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Contact> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating contact information");
        
        _context.Contacts.Update(request.Contact);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Contact information updated successfully");
        return request.Contact;
    }
}
