namespace PortfolioApi.Infrastructure.Services.Models;

public class EmailSettings
{
    public string SmtpHost { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string SmtpUser { get; set; } = string.Empty;
    public string SmtpPass { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;

    public string SmtpServer => SmtpHost;
    public string Username => SmtpUser;
    public string Password => SmtpPass;
    public string From => FromEmail;
}