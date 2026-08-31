using System.ComponentModel.DataAnnotations;

namespace caportal.Models.Entities;

public class HeroBannerSlide
{
    public int Id { get; set; }

    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Subtitle { get; set; }

    [MaxLength(100)]
    public string? Badge { get; set; }

    [MaxLength(500)]
    public string ImagePath { get; set; } = "/images/hero-banner.png";

    [MaxLength(500)]
    public string? MobileImagePath { get; set; }

    [MaxLength(500)]
    public string? LinkUrl { get; set; }

    [MaxLength(100)]
    public string? ButtonText { get; set; }

    [MaxLength(500)]
    public string? ButtonUrl { get; set; }

    [MaxLength(100)]
    public string? SecondaryButtonText { get; set; }

    [MaxLength(500)]
    public string? SecondaryButtonUrl { get; set; }

    public int DisplayOrder { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    [MaxLength(50)]
    public string SlideType { get; set; } = "image"; // "image" or "content"

    [MaxLength(50)]
    public string? BgGradientFrom { get; set; } = "#0a1628";

    [MaxLength(50)]
    public string? BgGradientTo { get; set; } = "#1a2a4a";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
