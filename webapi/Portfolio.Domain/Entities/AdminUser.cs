using Microsoft.AspNetCore.Identity;

namespace PortfolioApi.Domain.Entities;

public class AdminUser : IdentityUser<int>
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
