namespace caportal.Models;

public class CoveredService
{
    public int    Id           { get; set; }
    public string Icon         { get; set; } = "fas fa-briefcase"; // kept for legacy
    public string ImagePath    { get; set; } = "";                 // uploaded image URL
    public string Title        { get; set; } = string.Empty;
    public string Description  { get; set; } = string.Empty;
    public int    DisplayOrder { get; set; } = 0;
}
