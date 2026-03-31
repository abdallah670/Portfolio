using System;
using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Domain.Entities;

public class Hero {
    [Key] public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string HeadlineTop { get; set; } = string.Empty;
    public string HeadlineMain { get; set; } = string.Empty;
    public string AvailabilityLabel { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string HeroIntro { get; set; } = string.Empty;
    public string CtaPrimaryLabel { get; set; } = string.Empty;
    public string CtaPrimaryHref { get; set; } = string.Empty;
    public string CtaSecondaryLabel { get; set; } = string.Empty;
    public string CtaSecondaryHref { get; set; } = string.Empty;
    public string ProfileImage { get; set; } = string.Empty;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
