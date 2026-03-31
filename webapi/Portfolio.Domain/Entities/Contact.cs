using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Domain.Entities;

public class Contact {
    [Key] public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string WhatsApp { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
}
