namespace caportal.Models.Entities;

public class SubService
{
    public int Id { get; set; }
    public int CoveredServiceId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string ImagePath { get; set; } = string.Empty;
    public bool IsPopular { get; set; } = false;
    public bool IsNew { get; set; } = false;
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    // Navigation property
    public virtual CoveredService? CoveredService { get; set; }
}
